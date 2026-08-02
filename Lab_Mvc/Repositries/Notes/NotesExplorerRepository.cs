using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Notes;
using Models.Notes;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Notes
{
    public class NotesExplorerRepository : DapperRepositoryBase, INotesExplorerRepository
    {
        public NotesExplorerRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<NoteFolder> CreateFolder(NoteFolder folder)
        {
            var query = @"
                INSERT INTO Notes_Folders (UserId, ParentFolderId, FolderName, CreatedDate)
                VALUES (@UserId, @ParentFolderId, @FolderName, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (var conn = CreateConnection())
            {
                int insertedId = await conn.ExecuteScalarAsync<int>(query, folder);
                folder.FolderId = insertedId;
                return folder;
            }
        }

        public async Task<bool> UpdateFolder(NoteFolder folder)
        {
            var query = "UPDATE Notes_Folders SET FolderName = @FolderName, UpdatedDate = GETDATE() WHERE FolderId = @FolderId AND UserId = @UserId";
            using (var conn = CreateConnection())
            {
                int rows = await conn.ExecuteAsync(query, folder);
                return rows > 0;
            }
        }

        public async Task<bool> DeleteFolder(int folderId, int userId)
        {
            // Note: If folder contains pages or subfolders, DB foreign key should ideally restrict it or CASCADE delete.
            // Based on user requirements, they want a warning. So we just attempt delete. If it fails due to FK, controller can catch it, but we can also use Cascade or recursive delete if needed.
            // Actually, we can use a CTE or just delete and rely on Cascade if we set it up. I set up cascade for Pages but not SubFolders (NO ACTION on ParentFolderId).
            // Let's manually delete subfolders recursively or assume the user clears them first.
            
            var query = "DELETE FROM Notes_Folders WHERE FolderId = @FolderId AND UserId = @UserId";
            using (var conn = CreateConnection())
            {
                try
                {
                    int rows = await conn.ExecuteAsync(query, new { FolderId = folderId, UserId = userId });
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    // FK constraint violation means it has subfolders.
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY")) 
                        throw new Exception("Cannot delete folder because it contains subfolders. Please delete subfolders first.");
                    throw;
                }
            }
        }

        public async Task<NotePage> CreatePage(NotePage page)
        {
            var query = @"
                INSERT INTO Notes_Pages (FolderId, UserId, Title, Content, CreatedDate)
                VALUES (@FolderId, @UserId, @Title, @Content, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (var conn = CreateConnection())
            {
                int insertedId = await conn.ExecuteScalarAsync<int>(query, page);
                page.PageId = insertedId;
                return page;
            }
        }

        public async Task<bool> UpdatePage(NotePage page)
        {
            var query = "UPDATE Notes_Pages SET Title = @Title, Content = @Content, UpdatedDate = GETDATE() WHERE PageId = @PageId AND UserId = @UserId";
            using (var conn = CreateConnection())
            {
                int rows = await conn.ExecuteAsync(query, page);
                return rows > 0;
            }
        }

        public async Task<bool> DeletePage(int pageId, int userId)
        {
            var query = "DELETE FROM Notes_Pages WHERE PageId = @PageId AND UserId = @UserId";
            using (var conn = CreateConnection())
            {
                int rows = await conn.ExecuteAsync(query, new { PageId = pageId, UserId = userId });
                return rows > 0;
            }
        }

        public async Task<NotePage> GetPageById(int pageId, int userId)
        {
            var query = "SELECT * FROM Notes_Pages WHERE PageId = @PageId AND UserId = @UserId";
            using (var conn = CreateConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<NotePage>(query, new { PageId = pageId, UserId = userId });
            }
        }

        public async Task<IEnumerable<NoteFolder>> GetAllFoldersByUser(int userId)
        {
            var query = "SELECT * FROM Notes_Folders WHERE UserId = @UserId ORDER BY FolderName";
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<NoteFolder>(query, new { UserId = userId });
            }
        }

        public async Task<IEnumerable<NotePageDto>> GetAllPagesByUser(int userId)
        {
            var query = "SELECT PageId, FolderId, Title FROM Notes_Pages WHERE UserId = @UserId ORDER BY Title";
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<NotePageDto>(query, new { UserId = userId });
            }
        }

        public async Task<IEnumerable<NotePageDto>> SearchPages(int userId, string queryText)
        {
            var query = "SELECT PageId, FolderId, Title FROM Notes_Pages WHERE UserId = @UserId AND Title LIKE @Search ORDER BY Title";
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<NotePageDto>(query, new { UserId = userId, Search = $"%{queryText}%" });
            }
        }

        public async Task<IEnumerable<NoteFolder>> SearchFolders(int userId, string queryText)
        {
            var query = "SELECT * FROM Notes_Folders WHERE UserId = @UserId AND FolderName LIKE @Search ORDER BY FolderName";
            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<NoteFolder>(query, new { UserId = userId, Search = $"%{queryText}%" });
            }
        }
    }
}

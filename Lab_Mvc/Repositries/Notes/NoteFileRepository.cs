using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Notes;
using Models.Notes;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Notes
{
    public class NoteFileRepository : DapperRepositoryBase, INoteFileRepository
    {
        public NoteFileRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<NoteFile> UploadFile(NoteFile file)
        {
            var query = @"
                INSERT INTO Notes_Files
                    (FolderId, UserId, DisplayName, OriginalFileName, StoredFileName, Extension, MimeType, SizeBytes, StoragePath, FileData, DownloadCount, Status, CreatedDate)
                VALUES
                    (@FolderId, @UserId, @DisplayName, @OriginalFileName, @StoredFileName, @Extension, @MimeType, @SizeBytes, @StoragePath, @FileData, 0, 'Active', GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (var conn = CreateConnection())
            {
                using (var cmd = new SqlCommand(query, (SqlConnection)conn))
                {
                    cmd.Parameters.AddWithValue("@FolderId", file.FolderId);
                    cmd.Parameters.AddWithValue("@UserId", file.UserId);
                    cmd.Parameters.AddWithValue("@DisplayName", file.DisplayName);
                    cmd.Parameters.AddWithValue("@OriginalFileName", file.OriginalFileName);
                    cmd.Parameters.AddWithValue("@StoredFileName", file.StoredFileName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Extension", file.Extension);
                    cmd.Parameters.AddWithValue("@MimeType", file.MimeType);
                    cmd.Parameters.AddWithValue("@SizeBytes", file.SizeBytes);
                    cmd.Parameters.AddWithValue("@StoragePath", file.StoragePath ?? (object)DBNull.Value);
                    
                    var fileDataParam = new SqlParameter("@FileData", SqlDbType.VarBinary, -1);
                    if (file.FileData != null)
                        fileDataParam.Value = file.FileData;
                    else
                        fileDataParam.Value = DBNull.Value;
                    cmd.Parameters.Add(fileDataParam);

                    await ((SqlConnection)conn).OpenAsync();
                    var insertedId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    file.FileId = insertedId;
                    return file;
                }
            }
        }

        public async Task<IEnumerable<NoteFile>> GetFilesByFolder(int folderId, int userId)
        {
            var query = @"
                SELECT FileId, FolderId, UserId, DisplayName, OriginalFileName, StoredFileName, Extension, MimeType, SizeBytes, StoragePath, DownloadCount, Status, CreatedDate, UpdatedDate
                FROM Notes_Files
                WHERE FolderId = @FolderId AND UserId = @UserId AND Status = 'Active'
                ORDER BY DisplayName";

            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<NoteFile>(query, new { FolderId = folderId, UserId = userId });
            }
        }

        public async Task<IEnumerable<NoteFileDto>> GetAllFilesByUser(int userId)
        {
            var query = @"
                SELECT FileId, FolderId, DisplayName, Extension, MimeType, SizeBytes, DownloadCount, CreatedDate, UpdatedDate
                FROM Notes_Files
                WHERE UserId = @UserId AND Status = 'Active'
                ORDER BY DisplayName";

            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<NoteFileDto>(query, new { UserId = userId });
            }
        }

        public async Task<NoteFile?> GetFileById(int fileId, int userId)
        {
            var query = @"SELECT FileId, FolderId, UserId, DisplayName, OriginalFileName, StoredFileName, Extension, MimeType, SizeBytes, StoragePath, DownloadCount, Status, CreatedDate, UpdatedDate 
                          FROM Notes_Files WHERE FileId = @FileId AND UserId = @UserId AND Status = 'Active'";
            using (var conn = CreateConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<NoteFile>(query, new { FileId = fileId, UserId = userId });
            }
        }

        public async Task<bool> RenameFile(int fileId, int userId, string newDisplayName)
        {
            var query = @"
                UPDATE Notes_Files
                SET DisplayName = @DisplayName, UpdatedDate = GETDATE()
                WHERE FileId = @FileId AND UserId = @UserId AND Status = 'Active'";

            using (var conn = CreateConnection())
            {
                int rows = await conn.ExecuteAsync(query, new { DisplayName = newDisplayName, FileId = fileId, UserId = userId });
                return rows > 0;
            }
        }

        public async Task<NoteFile?> DeleteFile(int fileId, int userId)
        {
            // First retrieve metadata so caller knows what was deleted (if needed for physical cleanup during migration)
            var selectQuery = @"SELECT FileId, FolderId, UserId, DisplayName, OriginalFileName, StoredFileName, Extension, MimeType, SizeBytes, StoragePath, DownloadCount, Status, CreatedDate, UpdatedDate 
                                FROM Notes_Files WHERE FileId = @FileId AND UserId = @UserId AND Status = 'Active'";
            var deleteQuery = "DELETE FROM Notes_Files WHERE FileId = @FileId AND UserId = @UserId";

            using (var conn = CreateConnection())
            {
                var file = await conn.QuerySingleOrDefaultAsync<NoteFile>(selectQuery, new { FileId = fileId, UserId = userId });
                if (file == null) return null;

                await conn.ExecuteAsync(deleteQuery, new { FileId = fileId, UserId = userId });
                return file;
            }
        }

        public async Task IncrementDownloadCount(int fileId)
        {
            var query = "UPDATE Notes_Files SET DownloadCount = DownloadCount + 1 WHERE FileId = @FileId";
            using (var conn = CreateConnection())
            {
                await conn.ExecuteAsync(query, new { FileId = fileId });
            }
        }

        public async Task<IEnumerable<NoteFileDto>> SearchFiles(int userId, string query)
        {
            var sql = @"
                SELECT FileId, FolderId, DisplayName, Extension, MimeType, SizeBytes, DownloadCount, CreatedDate, UpdatedDate
                FROM Notes_Files
                WHERE UserId = @UserId AND Status = 'Active' AND DisplayName LIKE @Search
                ORDER BY DisplayName";

            using (var conn = CreateConnection())
            {
                return await conn.QueryAsync<NoteFileDto>(sql, new { UserId = userId, Search = $"%{query}%" });
            }
        }

        public async Task<bool> FolderHasFiles(int folderId, int userId)
        {
            var query = "SELECT COUNT(1) FROM Notes_Files WHERE FolderId = @FolderId AND UserId = @UserId AND Status = 'Active'";
            using (var conn = CreateConnection())
            {
                int count = await conn.ExecuteScalarAsync<int>(query, new { FolderId = folderId, UserId = userId });
                return count > 0;
            }
        }
    }
}

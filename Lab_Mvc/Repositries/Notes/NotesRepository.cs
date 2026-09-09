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
    public class NotesRepository : DapperRepositoryBase, INotesRepository
    {
        public NotesRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<NotesUser> RegisterUser(NotesUser user)
        {
            var checkQuery = "SELECT COUNT(1) FROM Notes_Users WHERE number = @Number";
            var insertQuery = @"
                INSERT INTO Notes_Users (name, number, password, created_at)
                VALUES (@Name, @Number, @Password, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (var conn = CreateConnection())
            {
                int exists = await conn.ExecuteScalarAsync<int>(checkQuery, new { Number = user.Number });
                if (exists > 0)
                {
                    throw new Exception("Mobile number is already registered.");
                }

                int insertedId = await conn.ExecuteScalarAsync<int>(insertQuery, user);
                user.Id = insertedId;
                return user;
            }
        }

        public async Task<NotesUser> LoginUser(string number, string password)
        {
            var query = "SELECT id, name, number, password, created_at FROM Notes_Users WHERE number = @Number AND password = @Password";
            using (var conn = CreateConnection())
            {
                var result = await conn.QuerySingleOrDefaultAsync<NotesUser>(query, new { Number = number, Password = password });
                return result;
            }
        }

        public async Task<NoteItem> CreateNote(NoteItem note)
        {
            var query = @"
                INSERT INTO Notes_Items (user_id, title, content, created_date)
                VALUES (@UserId, @Title, @Content, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (var conn = CreateConnection())
            {
                int insertedId = await conn.ExecuteScalarAsync<int>(query, note);
                note.Id = insertedId;
                return note;
            }
        }

        public async Task<IEnumerable<NoteItem>> GetNotesByUserId(int userId, string searchQuery = null)
        {
            var query = "SELECT id, user_id AS UserId, title, content, created_date AS CreatedDate FROM Notes_Items WHERE user_id = @UserId";
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query += " AND (title LIKE @Search OR content LIKE @Search)";
            }
            query += " ORDER BY created_date DESC";

            using (var conn = CreateConnection())
            {
                var searchPattern = $"%{searchQuery}%";
                return await conn.QueryAsync<NoteItem>(query, new { UserId = userId, Search = searchPattern });
            }
        }

        public async Task<bool> UpdateNote(NoteItem note)
        {
            var query = "UPDATE Notes_Items SET title = @Title, content = @Content WHERE id = @Id AND user_id = @UserId";
            using (var conn = CreateConnection())
            {
                int rows = await conn.ExecuteAsync(query, note);
                return rows > 0;
            }
        }

        public async Task<bool> DeleteNote(int noteId, int userId)
        {
            var query = "DELETE FROM Notes_Items WHERE id = @Id AND user_id = @UserId";
            using (var conn = CreateConnection())
            {
                int rows = await conn.ExecuteAsync(query, new { Id = noteId, UserId = userId });
                return rows > 0;
            }
        }
    }
}

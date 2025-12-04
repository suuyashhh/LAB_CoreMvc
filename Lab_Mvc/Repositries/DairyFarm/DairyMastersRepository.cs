using Dapper;
using Lab_Mvc.Contest; // where your DapperContext is
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class DairyMastersRepository : IDairyMasters
    {
        private readonly DapperContext _dapperContext;

        public DairyMastersRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        // Animals
        public async Task<IEnumerable<AnimalDto>> GetAnimalsByUser(int userId)
        {
            var sql = "SELECT animal_id AS AnimalId, user_id AS UserId, animal_name AS AnimalName, date FROM AnimalsName WHERE user_id = @userId ORDER BY animal_id DESC";
            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryAsync<AnimalDto>(sql, new { userId });
        }

        public async Task<int> CreateAnimal(AnimalDto animal)
        {
            var sql = "INSERT INTO AnimalsName (animal_name, user_id, date) VALUES (@AnimalName, @UserId, GETDATE()); SELECT CAST(SCOPE_IDENTITY() as int)";
            using var conn = _dapperContext.CreateConnection();
            var id = await conn.QuerySingleAsync<int>(sql, animal);
            return id;
        }

        public async Task<bool> UpdateAnimal(AnimalDto animal)
        {
            var sql = "UPDATE AnimalsName SET animal_name = @AnimalName WHERE animal_id = @AnimalId";
            using var conn = _dapperContext.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, animal);
            return rows > 0;
        }

        public async Task<bool> DeleteAnimal(int animalId)
        {
            var sql = "DELETE FROM AnimalsName WHERE animal_id = @animalId";
            using var conn = _dapperContext.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new { animalId });
            return rows > 0;
        }

        // Feeds
        public async Task<IEnumerable<FeedDto>> GetFeedsByUser(int userId)
        {
            var sql = "SELECT feed_id AS FeedId, user_id AS UserId, feed_name AS FeedName FROM Feeds WHERE user_id = @userId ORDER BY feed_id DESC";
            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryAsync<FeedDto>(sql, new { userId });
        }

        public async Task<int> CreateFeed(FeedDto feed)
        {
            var sql = "INSERT INTO Feeds (feed_name, user_id) VALUES (@FeedName, @UserId); SELECT CAST(SCOPE_IDENTITY() as int)";
            using var conn = _dapperContext.CreateConnection();
            var id = await conn.QuerySingleAsync<int>(sql, feed);
            return id;
        }

        public async Task<bool> UpdateFeed(FeedDto feed)
        {
            var sql = "UPDATE Feeds SET feed_name = @FeedName WHERE feed_id = @FeedId";
            using var conn = _dapperContext.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, feed);
            return rows > 0;
        }

        public async Task<bool> DeleteFeed(int feedId)
        {
            var sql = "DELETE FROM Feeds WHERE feed_id = @feedId";
            using var conn = _dapperContext.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new { feedId });
            return rows > 0;
        }
    }
}

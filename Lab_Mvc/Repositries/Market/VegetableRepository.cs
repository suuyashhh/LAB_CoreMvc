using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Market;
using Models.Market;
using Dapper;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Market
{
    public class VegetableRepository : DapperRepositoryBase, IVegetableRepository
    {
        public VegetableRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<Vegetable>> GetAllAsync()
        {
            var sql = "SELECT id, Eng_vegetable_name AS EngVegetableName, Mar_vegetable_name AS MarVegetableName FROM [dbo].[Market_Vegetable] ORDER BY Eng_vegetable_name";
            return await QueryAsync<Vegetable>(sql);
        }

        public async Task<Vegetable?> GetByIdAsync(int id)
        {
            var sql = "SELECT id, Eng_vegetable_name AS EngVegetableName, Mar_vegetable_name AS MarVegetableName FROM [dbo].[Market_Vegetable] WHERE id = @Id";
            return await QuerySingleOrDefaultAsync<Vegetable>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(Vegetable vegetable)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "INSERT INTO [dbo].[Market_Vegetable] (Eng_vegetable_name, Mar_vegetable_name) VALUES (@EngVegetableName, @MarVegetableName); SELECT SCOPE_IDENTITY();",
                new { EngVegetableName = vegetable.EngVegetableName, MarVegetableName = vegetable.MarVegetableName }
            );
        }

        public async Task<bool> UpdateAsync(Vegetable vegetable)
        {
            using var connection = CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(
                "UPDATE [dbo].[Market_Vegetable] SET Eng_vegetable_name = @EngVegetableName, Mar_vegetable_name = @MarVegetableName WHERE id = @id",
                new { id = vegetable.Id, EngVegetableName = vegetable.EngVegetableName, MarVegetableName = vegetable.MarVegetableName }
            );
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM [dbo].[Market_Vegetable] WHERE id = @id",
                new { id }
            );
            return rowsAffected > 0;
        }
    }
}

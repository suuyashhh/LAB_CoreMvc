using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Tejas;
using Models.Tejas;
using SmartParking.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Tejas
{
    public class TejasFoodRepository : DapperRepositoryBase, ITejasFood
    {
        public TejasFoodRepository(DapperContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TejasFoodItem>> GetAll()
        {
            var query = @"SELECT [Id], [Name], [Price], [Category], [Image], [Active] FROM [dbo].[Tejas_FoodItem]";
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<TejasFoodItem>(query);
            }
        }

        public async Task<TejasFoodItem?> GetById(string id)
        {
            var query = @"SELECT [Id], [Name], [Price], [Category], [Image], [Active] FROM [dbo].[Tejas_FoodItem] WHERE [Id] = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<TejasFoodItem>(query, new { Id = id });
            }
        }

        public async Task<int> Insert(TejasFoodItem item)
        {
            var query = @"
                INSERT INTO [dbo].[Tejas_FoodItem] ([Id], [Name], [Price], [Category], [Image], [Active])
                VALUES (@Id, @Name, @Price, @Category, @Image, @Active)";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(query, item);
            }
        }

        public async Task<int> Update(TejasFoodItem item)
        {
            var query = @"
                UPDATE [dbo].[Tejas_FoodItem]
                SET [Name] = @Name,
                    [Price] = @Price,
                    [Category] = @Category,
                    [Image] = @Image,
                    [Active] = @Active
                WHERE [Id] = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(query, item);
            }
        }

        public async Task<int> Delete(string id)
        {
            var query = @"DELETE FROM [dbo].[Tejas_FoodItem] WHERE [Id] = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}

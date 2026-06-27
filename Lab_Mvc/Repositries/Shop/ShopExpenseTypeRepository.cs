using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Shop;
using Models.Shop;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Shop
{
    public class ShopExpenseTypeRepository : IShopExpenseType
    {
        private readonly DapperContext _context;

        public ShopExpenseTypeRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DTOShopExpenseType>> GetAll()
        {
            var query = @"
                SELECT 
                    [EX_ID],
                    [NAME]
                FROM [dbo].[SHOP_EXPENSE_TYPE]
                ORDER BY [EX_ID] DESC";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<DTOShopExpenseType>(query);
                return result;
            }
        }

        public async Task<DTOShopExpenseType?> GetById(int exId)
        {
            var query = @"
                SELECT 
                    [EX_ID],
                    [NAME]
                FROM [dbo].[SHOP_EXPENSE_TYPE]
                WHERE [EX_ID] = @ExId";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<DTOShopExpenseType>(
                    query,
                    new { ExId = exId }
                );
                return result;
            }
        }

        public async Task<int> Insert(DTOShopExpenseType model)
        {
            var query = @"
                INSERT INTO [dbo].[SHOP_EXPENSE_TYPE] ([NAME])
                VALUES (@NAME);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, model);
                return id;
            }
        }

        public async Task<int> Update(DTOShopExpenseType model)
        {
            var query = @"
                UPDATE [dbo].[SHOP_EXPENSE_TYPE]
                SET [NAME] = @NAME
                WHERE [EX_ID] = @EX_ID";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, model);
                return affectedRows;
            }
        }

        public async Task<int> Delete(int exId)
        {
            var query = @"
                DELETE FROM [dbo].[SHOP_EXPENSE_TYPE]
                WHERE [EX_ID] = @ExId";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(
                    query,
                    new { ExId = exId }
                );
                return affectedRows;
            }
        }
    }
}

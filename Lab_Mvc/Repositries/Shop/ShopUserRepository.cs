using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Shop;
using Models.Shop;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Shop
{
    public class ShopUserRepository : IShopUser
    {
        private readonly DapperContext _context;

        public ShopUserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DTOShopLogin>> GetAll()
        {
            var query = @"
                SELECT 
                    [USER_ID],
                    [USER_NAME],
                    [PASS],
                    [CONTACT]
                FROM [dbo].[SHOP_USER]
                ORDER BY [USER_ID] DESC";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<DTOShopLogin>(query);
                return result;
            }
        }

        public async Task<DTOShopLogin?> GetById(long userId)
        {
            var query = @"
                SELECT 
                    [USER_ID],
                    [USER_NAME],
                    [PASS],
                    [CONTACT]
                FROM [dbo].[SHOP_USER]
                WHERE [USER_ID] = @UserId";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<DTOShopLogin>(
                    query,
                    new { UserId = userId }
                );
                return result;
            }
        }

        public async Task<long> Insert(DTOShopLogin model)
        {
            var query = @"
                INSERT INTO [dbo].[SHOP_USER] ([USER_NAME], [PASS], [CONTACT])
                VALUES (@USER_NAME, @PASS, @CONTACT);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<long>(query, model);
                return id;
            }
        }

        public async Task<int> Update(DTOShopLogin model)
        {
            var query = @"
                UPDATE [dbo].[SHOP_USER]
                SET [USER_NAME] = @USER_NAME,
                    [PASS] = @PASS,
                    [CONTACT] = @CONTACT
                WHERE [USER_ID] = @USER_ID";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, model);
                return affectedRows;
            }
        }

        public async Task<int> Delete(long userId)
        {
            var query = @"
                DELETE FROM [dbo].[SHOP_USER]
                WHERE [USER_ID] = @UserId";

            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(
                    query,
                    new { UserId = userId }
                );
                return affectedRows;
            }
        }
    }
}

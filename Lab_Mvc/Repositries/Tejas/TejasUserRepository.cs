using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Tejas;
using Models.Tejas;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Tejas
{
    public class TejasUserRepository : DapperRepositoryBase, ITejasUser
    {
        public TejasUserRepository(DapperContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DTOTejasLogin>> GetAll(long? shopId = null)
        {
            var query = @"
                SELECT 
                    u.[USER_ID],
                    u.[USER_NAME],
                    u.[PASS],
                    u.[CONTACT],
                    u.[USER_IMG],
                    u.[ROLE],
                    u.[TEJAS_SHOPES_ID],
                    s.[SHOP_NAME]
                FROM [dbo].[Tejas_USER] u
                LEFT JOIN [dbo].[Tejas_Shopes] s ON u.[TEJAS_SHOPES_ID] = s.[TEJAS_SHOPES_ID]
                WHERE u.[ACTIVE] = 'Y'
                  AND (@ShopId IS NULL OR u.[TEJAS_SHOPES_ID] = @ShopId)
                ORDER BY u.[USER_ID] DESC";

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<DTOTejasLogin>(query, new { ShopId = shopId });
                return result;
            }
        }

        public async Task<DTOTejasLogin?> GetById(long userId)
        {
            var query = @"
                SELECT 
                    u.[USER_ID],
                    u.[USER_NAME],
                    u.[PASS],
                    u.[CONTACT],
                    u.[USER_IMG],
                    u.[ROLE],
                    u.[TEJAS_SHOPES_ID],
                    s.[SHOP_NAME]
                FROM [dbo].[Tejas_USER] u
                LEFT JOIN [dbo].[Tejas_Shopes] s ON u.[TEJAS_SHOPES_ID] = s.[TEJAS_SHOPES_ID]
                WHERE u.[USER_ID] = @UserId";

            using (var connection = CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<DTOTejasLogin>(
                    query,
                    new { UserId = userId }
                );
                return result;
            }
        }

        public async Task<long> Insert(DTOTejasLogin model)
        {
            var query = @"
                INSERT INTO [dbo].[Tejas_USER] ([USER_NAME], [PASS], [CONTACT], [USER_IMG],[ROLE],[ACTIVE],[TEJAS_SHOPES_ID])
                VALUES (@USER_NAME, @PASS, @CONTACT, @USER_IMG, @ROLE, 'Y', @TEJAS_SHOPES_ID);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            using (var connection = CreateConnection())
            {
                var id = await connection.QuerySingleAsync<long>(query, model);
                return id;
            }
        }

        public async Task<int> Update(DTOTejasLogin model)
        {
            var query = @"
                UPDATE [dbo].[Tejas_USER]
                SET [USER_NAME] = @USER_NAME,
                    [PASS] = @PASS,
                    [CONTACT] = @CONTACT,
                    [USER_IMG] = @USER_IMG,
                    [ROLE] = @ROLE,
                    [TEJAS_SHOPES_ID] = @TEJAS_SHOPES_ID
                WHERE [USER_ID] = @USER_ID";

            using (var connection = CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, model);
                return affectedRows;
            }
        }

        public async Task<int> Delete(long userId)
        {
            var query = @"
                DELETE FROM [dbo].[Tejas_USER]
                WHERE [USER_ID] = @UserId";

            using (var connection = CreateConnection())
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

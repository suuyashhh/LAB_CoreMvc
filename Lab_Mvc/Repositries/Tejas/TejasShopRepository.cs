using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Tejas;
using Models.Tejas;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Tejas
{
    public class TejasShopRepository : DapperRepositoryBase, ITejasShop
    {
        public TejasShopRepository(DapperContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DTOTejasShop>> GetAll()
        {
            var query = @"
                SELECT 
                    [TEJAS_SHOPES_ID],
                    [SHOP_NAME],
                    [SHOP_CODE],
                    [ADDRESS],
                    [CONTACT],
                    [EMAIL],
                    [GST_NO],
                    [LOGO_URL],
                    [ACTIVE],
                    [CREATED_AT],
                    [UPDATED_AT]
                FROM [dbo].[Tejas_Shopes]
                ORDER BY [TEJAS_SHOPES_ID] ASC";

            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<DTOTejasShop>(query);
            }
        }

        public async Task<DTOTejasShop?> GetById(long shopId)
        {
            var query = @"
                SELECT 
                    [TEJAS_SHOPES_ID],
                    [SHOP_NAME],
                    [SHOP_CODE],
                    [ADDRESS],
                    [CONTACT],
                    [EMAIL],
                    [GST_NO],
                    [LOGO_URL],
                    [ACTIVE],
                    [CREATED_AT],
                    [UPDATED_AT]
                FROM [dbo].[Tejas_Shopes]
                WHERE [TEJAS_SHOPES_ID] = @ShopId";

            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<DTOTejasShop>(query, new { ShopId = shopId });
            }
        }

        public async Task<long> Insert(DTOTejasShop model)
        {
            var query = @"
                INSERT INTO [dbo].[Tejas_Shopes] 
                (
                    [SHOP_NAME],
                    [SHOP_CODE],
                    [ADDRESS],
                    [CONTACT],
                    [EMAIL],
                    [GST_NO],
                    [LOGO_URL],
                    [ACTIVE],
                    [CREATED_AT]
                )
                VALUES 
                (
                    @SHOP_NAME,
                    @SHOP_CODE,
                    @ADDRESS,
                    @CONTACT,
                    @EMAIL,
                    @GST_NO,
                    @LOGO_URL,
                    ISNULL(@ACTIVE, 'Y'),
                    GETDATE()
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            using (var connection = CreateConnection())
            {
                var id = await connection.QuerySingleAsync<long>(query, model);
                return id;
            }
        }

        public async Task<int> Update(DTOTejasShop model)
        {
            var query = @"
                UPDATE [dbo].[Tejas_Shopes]
                SET 
                    [SHOP_NAME] = @SHOP_NAME,
                    [SHOP_CODE] = @SHOP_CODE,
                    [ADDRESS] = @ADDRESS,
                    [CONTACT] = @CONTACT,
                    [EMAIL] = @EMAIL,
                    [GST_NO] = @GST_NO,
                    [LOGO_URL] = @LOGO_URL,
                    [ACTIVE] = @ACTIVE,
                    [UPDATED_AT] = GETDATE()
                WHERE [TEJAS_SHOPES_ID] = @TEJAS_SHOPES_ID";

            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(query, model);
            }
        }

        public async Task<int> Delete(long shopId)
        {
            // Soft delete or hard delete
            var query = @"
                UPDATE [dbo].[Tejas_Shopes]
                SET [ACTIVE] = 'N', [UPDATED_AT] = GETDATE()
                WHERE [TEJAS_SHOPES_ID] = @ShopId";

            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(query, new { ShopId = shopId });
            }
        }
    }
}

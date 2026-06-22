using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Shop;
using Models.Shop;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Shop
{
    public class ShopEntryRepository : IShopEntry
    {
        private readonly DapperContext _dapperContext;

        public ShopEntryRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<DTOShopEntry>> GetAll(long userId, bool isPaid)
        {
            var query = @"
                SELECT 
                    [SHOP_ENTRY_ID],
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                FROM [dbo].[SHOP_ENTRY]
                WHERE [USER_ID] = @UserId AND [IS_PAID] = @IsPaid
                ORDER BY [DATE] DESC, [SHOP_ENTRY_ID] DESC";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<DTOShopEntry>(
                    query,
                    new { UserId = userId, IsPaid = isPaid }
                );
                return result;
            }
        }

        public async Task<IEnumerable<DTOShopEntry>> GetAllTypesEntrys(long userId, System.DateTime? fromDate = null, System.DateTime? toDate = null)
        {
            var query = @"
                SELECT 
                    [SHOP_ENTRY_ID],
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                FROM [dbo].[SHOP_ENTRY]
                WHERE [USER_ID] = @UserId 
                  AND (@FromDate IS NULL OR [DATE] >= @FromDate)
                  AND (@ToDate IS NULL OR [DATE] <= @ToDate)
                ORDER BY [DATE] DESC, [SHOP_ENTRY_ID] DESC";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<DTOShopEntry>(
                    query,
                    new { UserId = userId, FromDate = fromDate, ToDate = toDate }
                );
                return result;
            }
        }

        public async Task<DTOShopEntry> GetById(long shopEntryId, long userId)
        {
            var query = @"
                SELECT 
                    [SHOP_ENTRY_ID],
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                FROM [dbo].[SHOP_ENTRY]
                WHERE [SHOP_ENTRY_ID] = @ShopEntryId
                AND [USER_ID] = @UserId";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<DTOShopEntry>(
                    query,
                    new { ShopEntryId = shopEntryId, UserId = userId }
                );
                return result;
            }
        }

        public async Task<long> Insert(DTOShopEntry model)
        {
            var query = @"
                DECLARE @NewShopEntryId BIGINT;
                
                -- Get the next SHOP_ENTRY_ID for this user
                SELECT @NewShopEntryId = ISNULL(MAX([SHOP_ENTRY_ID]), 0) + 1
                FROM [dbo].[SHOP_ENTRY]
                WHERE [USER_ID] = @USER_ID;

                -- Insert the new record
                INSERT INTO [dbo].[SHOP_ENTRY]
                (
                    [SHOP_ENTRY_ID],
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                )
                VALUES
                (
                    @NewShopEntryId,
                    @IS_PAID,
                    @REASON,
                    @PRICE,
                    @USER_ID,
                    @IMAGE1,
                    @IMAGE2,
                    @IMAGE3,
                    @IMAGE4,
                    @DATE
                );

                SELECT @NewShopEntryId;";

            using (var connection = _dapperContext.CreateConnection())
            {
                var newId = await connection.ExecuteScalarAsync<long>(query, model);
                return newId;
            }
        }

        public async Task<int> Update(DTOShopEntry model)
        {
            var query = @"
                UPDATE [dbo].[SHOP_ENTRY]
                SET 
                    [IS_PAID] = @IS_PAID,
                    [REASON] = @REASON,
                    [PRICE] = @PRICE,
                    [IMAGE1] = @IMAGE1,
                    [IMAGE2] = @IMAGE2,
                    [IMAGE3] = @IMAGE3,
                    [IMAGE4] = @IMAGE4,
                    [DATE] = @DATE
                WHERE [SHOP_ENTRY_ID] = @SHOP_ENTRY_ID
                AND [USER_ID] = @USER_ID";

            using (var connection = _dapperContext.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, model);
                return affectedRows;
            }
        }

        public async Task<int> Delete(long shopEntryId, long userId)
        {
            var query = @"
                DELETE FROM [dbo].[SHOP_ENTRY]
                WHERE [SHOP_ENTRY_ID] = @ShopEntryId
                AND [USER_ID] = @UserId";

            using (var connection = _dapperContext.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(
                    query,
                    new { ShopEntryId = shopEntryId, UserId = userId }
                );
                return affectedRows;
            }
        }
    }
}

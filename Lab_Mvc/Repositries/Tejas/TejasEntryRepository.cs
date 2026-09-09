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
    public class TejasEntryRepository : DapperRepositoryBase, ITejasEntry
    {
        public TejasEntryRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<DTOTejasEntry>> GetAll(long userId, bool isPaid, long? shopId = null)
        {
            var query = @"
                SELECT 
                    [TEJAS_ENTRY_ID] AS TEJAS_ENTRY_ID,
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE],
                    [EntryType],
                    [TEJAS_SHOPES_ID]
                FROM [dbo].[Tejas_ENTRY]
                WHERE [IS_PAID] = @IsPaid
                  AND (@ShopId IS NULL OR [TEJAS_SHOPES_ID] = @ShopId)
                ORDER BY [DATE] DESC, [TEJAS_ENTRY_ID] DESC";

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<DTOTejasEntry>(
                    query,
                    new { IsPaid = isPaid, ShopId = shopId }
                );
                return result;
            }
        }

        public async Task<IEnumerable<DTOTejasEntry>> GetAllTypesEntrys(long userId, System.DateTime? fromDate = null, System.DateTime? toDate = null, long? shopId = null)
        {
            var query = @"
                SELECT 
                    [TEJAS_ENTRY_ID] AS TEJAS_ENTRY_ID,
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE],
                    [EntryType],
                    [TEJAS_SHOPES_ID]
                FROM [dbo].[Tejas_ENTRY]
                WHERE (@FromDate IS NULL OR [DATE] >= @FromDate)
                  AND (@ToDate IS NULL OR [DATE] <= @ToDate)
                  AND (@ShopId IS NULL OR [TEJAS_SHOPES_ID] = @ShopId)
                ORDER BY [DATE] DESC, [TEJAS_ENTRY_ID] DESC";

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<DTOTejasEntry>(
                    query,
                    new { FromDate = fromDate, ToDate = toDate, ShopId = shopId }
                );
                return result;
            }
        }

        public async Task<DTOTejasEntry> GetById(long tejasEntryId, long userId)
        {
            var query = @"
                SELECT 
                    [TEJAS_ENTRY_ID] AS TEJAS_ENTRY_ID,
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE],
                    [EntryType],
                    [TEJAS_SHOPES_ID]
                FROM [dbo].[Tejas_ENTRY]
                WHERE [TEJAS_ENTRY_ID] = @TejasEntryId";

            using (var connection = CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<DTOTejasEntry>(
                    query,
                    new { TejasEntryId = tejasEntryId }
                );
                return result;
            }
        }

        public async Task<long> Insert(DTOTejasEntry model)
        {
            var query = @"
                DECLARE @NewTejasEntryId BIGINT;
                
                -- Get the next Tejas_ENTRY_ID
                SELECT @NewTejasEntryId = ISNULL(MAX([TEJAS_ENTRY_ID]), 0) + 1
                FROM [dbo].[Tejas_ENTRY];

                -- Insert the new record
                INSERT INTO [dbo].[Tejas_ENTRY]
                (
                    [TEJAS_ENTRY_ID],
                    [IS_PAID],
                    [REASON],
                    [PRICE],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE],
                    [EntryType],
                    [TEJAS_SHOPES_ID]
                )
                VALUES
                (
                    @NewTejasEntryId,
                    @IS_PAID,
                    @REASON,
                    @PRICE,
                    @USER_ID,
                    @IMAGE1,
                    @IMAGE2,
                    @IMAGE3,
                    @IMAGE4,
                    @DATE,
                    @EntryType,
                    @TEJAS_SHOPES_ID
                );

                SELECT @NewTejasEntryId;";

            using (var connection = CreateConnection())
            {
                var newId = await connection.ExecuteScalarAsync<long>(query, model);
                return newId;
            }
        }

        public async Task<int> Update(DTOTejasEntry model)
        {
            var query = @"
                UPDATE [dbo].[Tejas_ENTRY]
                SET 
                    [IS_PAID] = @IS_PAID,
                    [REASON] = @REASON,
                    [PRICE] = @PRICE,
                    [IMAGE1] = @IMAGE1,
                    [IMAGE2] = @IMAGE2,
                    [IMAGE3] = @IMAGE3,
                    [IMAGE4] = @IMAGE4,
                    [DATE] = @DATE,
                    [EntryType] = @EntryType,
                    [TEJAS_SHOPES_ID] = @TEJAS_SHOPES_ID
                WHERE [TEJAS_ENTRY_ID] = @TEJAS_ENTRY_ID";

            using (var connection = CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, model);
                return affectedRows;
            }
        }

        public async Task<int> Delete(long tejasEntryId, long userId)
        {
            var query = @"
                DELETE FROM [dbo].[Tejas_ENTRY]
                WHERE [Tejas_ENTRY_ID] = @TejasEntryId";

            using (var connection = CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(
                    query,
                    new { TejasEntryId = tejasEntryId }
                );
                return affectedRows;
            }
        }
    }
}

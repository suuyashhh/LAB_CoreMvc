using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Farm;
using Models.Farm;
using System.Data;

namespace Lab_Mvc.Repositries.Farm
{
    public class FarmEntryRepository : IFarmEntry
    {
        private readonly DapperContext _dapperContext;

        public FarmEntryRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<DTOFarmEntry>> GetAllTypesEntrys(long farmId, long userId)
        {
            var query = @"
                SELECT 
                    [FARM_ENTRY_ID],
                    [ENTRY_TYPE],
                    [REASON],
                    [PRICE],
                    [FARM_ID],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                FROM [dbo].[FARM_ENTRY]
                WHERE [FARM_ID] = @FarmId 
                AND [USER_ID] = @UserId 
                ORDER BY [DATE] DESC, [FARM_ENTRY_ID] DESC";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<DTOFarmEntry>(
                    query,
                    new { FarmId = farmId, UserId = userId}
                );
                return result;
            }
        }

        public async Task<IEnumerable<DTOFarmEntry>> GetAll(long farmId, long userId,string entryTypeName)
        {
            var query = @"
                SELECT 
                    [FARM_ENTRY_ID],
                    [ENTRY_TYPE],
                    [REASON],
                    [PRICE],
                    [FARM_ID],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                FROM [dbo].[FARM_ENTRY]
                WHERE [FARM_ID] = @FarmId 
                AND [USER_ID] = @UserId AND ENTRY_TYPE =@EntryType
                ORDER BY [DATE] DESC, [FARM_ENTRY_ID] DESC";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<DTOFarmEntry>(
                    query,
                    new { FarmId = farmId, UserId = userId, EntryType = entryTypeName }
                );
                return result;
            }
        }

        public async Task<DTOFarmEntry> GetById(long farmEntryId, long farmId, long userId)
        {
            var query = @"
                SELECT 
                    [FARM_ENTRY_ID],
                    [ENTRY_TYPE],
                    [REASON],
                    [PRICE],
                    [FARM_ID],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                FROM [dbo].[FARM_ENTRY]
                WHERE [FARM_ENTRY_ID] = @FarmEntryId
                AND [FARM_ID] = @FarmId 
                AND [USER_ID] = @UserId";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<DTOFarmEntry>(
                    query,
                    new { FarmEntryId = farmEntryId, FarmId = farmId, UserId = userId }
                );
                return result;
            }
        }

        public async Task<long> Insert(DTOFarmEntry model)
        {
            var query = @"
                DECLARE @NewFarmEntryId BIGINT;
                
                -- Get the next FARM_ENTRY_ID for this farm and user
                SELECT @NewFarmEntryId = ISNULL(MAX([FARM_ENTRY_ID]), 0) + 1
                FROM [dbo].[FARM_ENTRY]
                WHERE [FARM_ID] = @FARM_ID 
                AND [USER_ID] = @USER_ID;

                -- Insert the new record
                INSERT INTO [dbo].[FARM_ENTRY]
                (
                    [FARM_ENTRY_ID],
                    [ENTRY_TYPE],
                    [REASON],
                    [PRICE],
                    [FARM_ID],
                    [USER_ID],
                    [IMAGE1],
                    [IMAGE2],
                    [IMAGE3],
                    [IMAGE4],
                    [DATE]
                )
                VALUES
                (
                    @NewFarmEntryId,
                    @ENTRY_TYPE,
                    @REASON,
                    @PRICE,
                    @FARM_ID,
                    @USER_ID,
                    @IMAGE1,
                    @IMAGE2,
                    @IMAGE3,
                    @IMAGE4,
                    @DATE
                );

                SELECT @NewFarmEntryId;";

            using (var connection = _dapperContext.CreateConnection())
            {
                var newId = await connection.ExecuteScalarAsync<long>(query, model);
                return newId;
            }
        }

        public async Task<int> Update(DTOFarmEntry model)
        {
            var query = @"
                UPDATE [dbo].[FARM_ENTRY]
                SET 
                    [ENTRY_TYPE] = @ENTRY_TYPE,
                    [REASON] = @REASON,
                    [PRICE] = @PRICE,
                    [IMAGE1] = @IMAGE1,
                    [IMAGE2] = @IMAGE2,
                    [IMAGE3] = @IMAGE3,
                    [IMAGE4] = @IMAGE4,
                    [DATE] = @DATE
                WHERE [FARM_ENTRY_ID] = @FARM_ENTRY_ID
                AND [FARM_ID] = @FARM_ID
                AND [USER_ID] = @USER_ID";

            using (var connection = _dapperContext.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(query, model);
                return affectedRows;
            }
        }

        public async Task<int> Delete(long farmEntryId, long farmId, long userId)
        {
            var query = @"
                DELETE FROM [dbo].[FARM_ENTRY]
                WHERE [FARM_ENTRY_ID] = @FarmEntryId
                AND [FARM_ID] = @FarmId
                AND [USER_ID] = @UserId";

            using (var connection = _dapperContext.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(
                    query,
                    new { FarmEntryId = farmEntryId, FarmId = farmId, UserId = userId }
                );
                return affectedRows;
            }
        }
    }
}
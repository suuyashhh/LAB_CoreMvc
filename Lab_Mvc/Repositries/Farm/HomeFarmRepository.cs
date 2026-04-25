using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Farm;
using Models.Farm;
using System.Data;

namespace Lab_Mvc.Repositries.Farm
{
    public class HomeFarmRepository : IHomeFarm
    {
        private readonly DapperContext _dapperContext;

        public HomeFarmRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<DTOHomeFarm>> GetAll(string userId)
        {
            var query = @"SELECT FARM_ID, FARM_NAME, USER_ID, IMAGE 
                          FROM FARM_NAMES
                          WHERE USER_ID = @USER_ID AND STATUS_CODE = 0";

            using var connection = _dapperContext.CreateConnection();
            return await connection.QueryAsync<DTOHomeFarm>(query, new { USER_ID = userId });
        }

        public async Task<int> Insert(DTOHomeFarm model)
        {
            try
            {
                var query = @"
            DECLARE @NewFarmId INT;
            
            SELECT @NewFarmId = ISNULL(MAX(FARM_ID), 0) + 1 
            FROM FARM_NAMES 
            WHERE USER_ID = @USER_ID;
            
            INSERT INTO FARM_NAMES (FARM_ID, FARM_NAME, USER_ID, IMAGE, STATUS_CODE)
            VALUES (@NewFarmId, @FARM_NAME, @USER_ID, @IMAGE, 0);
            
            SELECT @NewFarmId;";

                using var connection = _dapperContext.CreateConnection();

                var newFarmId = await connection.ExecuteScalarAsync<int>(query, model);

                return newFarmId;   // better than returning 1
            }
            catch (Exception ex)
            {
                // Optional: log error here
                // _logger.LogError(ex, "Error inserting farm");

                throw new Exception("Error while inserting farm record", ex);
            }
        }


        public async Task<int> Update(DTOHomeFarm model)
        {
            var query = @"UPDATE FARM_NAMES 
                          SET FARM_NAME = @FARM_NAME,
                              IMAGE = @IMAGE
                          WHERE FARM_ID = @FARM_ID 
                            AND USER_ID = @USER_ID";

            using var connection = _dapperContext.CreateConnection();
            return await connection.ExecuteAsync(query, model);
        }

        public async Task<int> Delete(int farmId, string userId)
        {
            var query = @"UPDATE FARM_NAMES 
                          SET STATUS_CODE = 1
                          WHERE FARM_ID = @FARM_ID 
                            AND USER_ID = @USER_ID";

            using var connection = _dapperContext.CreateConnection();
            return await connection.ExecuteAsync(query, new
            {
                FARM_ID = farmId,
                USER_ID = userId
            });
        }

        // Optional: Helper method to get the next FARM_ID for a user
        public async Task<int> GetNextFarmId(string userId)
        {
            var query = @"SELECT ISNULL(MAX(FARM_ID), 0) + 1 
                          FROM FARM_NAMES 
                          WHERE USER_ID = @USER_ID";

            using var connection = _dapperContext.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, new { USER_ID = userId });
        }
    }
}
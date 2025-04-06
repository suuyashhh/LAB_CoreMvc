using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;
using System.Numerics;

namespace Lab_Mvc.Repositries
{
    public class TestRepository : ITest
    {
        private readonly DapperContext context;

        public TestRepository(DapperContext context)
        {
            //_dbContext = dBContext;
            this.context = context;
        }
        public async Task<IEnumerable<DTOTest>> GetTests()
        {
            try
            {
                var query = QueryConstant.GetTests;

                using (var connection = context.CreateConnection())
                {
                    var tests = await connection.QueryAsync<DTOTest>(query);
                    return tests.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<DTOTest>> GetTestById(Int64 test_code)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetTestById);
                parameters.Add("@TEST_CODE", test_code);

                using (var connection = context.CreateConnection())
                {
                    var tests = await connection.QueryAsync<DTOTest>(query, parameters);
                    return tests.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveTest(DTOTest test)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertTest);
                parameters.Add("@TEST_CODE", test.TEST_CODE);
                parameters.Add("@TEST_NAME", test.TEST_NAME);
                parameters.Add("@PRICE", test.PRICE);
                parameters.Add("@LAB_PRICE", test.LAB_PRICE);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                    //return await property;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task EditTest(DTOTest test,long test_code)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateTest);
                parameters.Add("@TEST_CODE", test_code);
                parameters.Add("@TEST_NAME", test.TEST_NAME);
                parameters.Add("@PRICE", test.PRICE);
                parameters.Add("@LAB_PRICE", test.LAB_PRICE);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                    //return await property;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteTest(long test_code)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteTest);
                parameters.Add("@TEST_CODE", test_code);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                    //return await property;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

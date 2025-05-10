using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

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


                Int64 newTestId = await GenerateTestId(test.COM_ID);


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertTest);
                parameters.Add("@TEST_CODE", newTestId);
                parameters.Add("@TEST_NAME", test.TEST_NAME);
                parameters.Add("@PRICE", test.PRICE);
                parameters.Add("@LAB_PRICE", test.LAB_PRICE);
                parameters.Add("@COM_ID", test.COM_ID);
                parameters.Add("@CRT_BY", test.CRT_BY);
                parameters.Add("@STATUS_CODE", test.STATUS_CODE);



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

        private async Task<long> GenerateTestId(string comId)
        {
            string fixedPart = "2";
            string fixedPartSec = comId;
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 DOCTOR_CODE FROM MST_DOCTOR WHERE DOCTOR_CODE LIKE @likePattern ORDER BY DOCTOR_CODE DESC";

            using (var connection = context.CreateConnection())
            {
                string lastId = await connection.ExecuteScalarAsync<string>(query, new { likePattern });

                int nextNumber = 1;
                if (!string.IsNullOrEmpty(lastId) && lastId.StartsWith(fixedPart + fixedPartSec))
                {
                    int prefixLength = (fixedPart + fixedPartSec).Length;
                    int lastNumber = int.Parse(lastId.Substring(prefixLength));
                    nextNumber = lastNumber + 1;
                }

                long newDoctorId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newDoctorId;
            }
        }


    }
}

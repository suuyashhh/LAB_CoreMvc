using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class BikeFuleRepository : IBikeFule
    {
        private readonly DapperContext context;

        public BikeFuleRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTOBikeFule>> GetBikeFule(int comId)
        {
            try
            {
                var query = QueryConstant.sp;

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetBikeFule);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var BikeFule = await connection.QueryAsync<DTOBikeFule>(query, parameters, commandType: CommandType.StoredProcedure);
                    return BikeFule.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<DTOBikeFule> GetBikeFuleById(long bike_id, int comId)
        {
            try
            {
                var query = "dbo.sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetBikeFuleById);
                parameters.Add("@BIKE_ID", bike_id);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var BikeFule = await connection.QuerySingleAsync<DTOBikeFule>(query, parameters, commandType: CommandType.StoredProcedure);
                    return BikeFule;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DTOBikeFule>> GetDateWiseBikeFule(string from_date, string to_date, int comId)
        {
            try
            {
                var query = QueryConstant.sp;
                using (var connection = context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", QueryConstant.GetDateWiseBikeFule);
                    parameters.Add("@From_Date", from_date);
                    parameters.Add("@To_Date", to_date);
                    parameters.Add("@COM_ID", comId);

                    using (var multi = await connection.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure))
                    {
                        var casepapers = (await multi.ReadAsync<DTOBikeFule>()).ToList();
                        return casepapers;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching case paper data", ex);
            }
        }
        public async Task SaveBikeFule(DTOBikeFule objBike)
        {
            try
            {
                var query = QueryConstant.sp;


                Int64 newBikeFuleId = await GenerateBikeFuleId(objBike.COM_ID);

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertBikeFule);
                parameters.Add("@BIKE_ID", newBikeFuleId);
                parameters.Add("@BIKE_NAME", objBike.BIKE_NAME);
                parameters.Add("@BIKE_PRICE", objBike.BIKE_PRICE);
                parameters.Add("@DATE", objBike.DATE);
                parameters.Add("@COM_ID", objBike.COM_ID);
                parameters.Add("@CRT_BY", objBike.CRT_BY);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task EditBikeFule(DTOBikeFule objBike, long bike_id)
        {
            try
            {
                var query = QueryConstant.sp;


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateBikeFule);
                parameters.Add("@BIKE_ID", bike_id);
                parameters.Add("@BIKE_NAME", objBike.BIKE_NAME);
                parameters.Add("@BIKE_PRICE", objBike.BIKE_PRICE);
                parameters.Add("@DATE", objBike.DATE);



                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteBikeFule(long bike_id, int comId)
        {
            try
            {
                var query = QueryConstant.sp;


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteBikeFule);
                parameters.Add("@BIKE_ID", bike_id);
                parameters.Add("@COM_ID", comId);


                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<long> GenerateBikeFuleId(string comId)
        {
            string fixedPart = "202";
            string fixedPartSec = comId.ToString();
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 BIKE_ID FROM MST_BIKE_FULE WHERE BIKE_ID LIKE @likePattern ORDER BY BIKE_ID DESC";

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

                long newBikeFuleId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newBikeFuleId;
            }
        }


    }
}

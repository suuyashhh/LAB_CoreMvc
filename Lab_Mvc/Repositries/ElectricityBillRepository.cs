using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class ElectricityBillRepository : IElectricityBill
    {
        private readonly DapperContext context;

        public ElectricityBillRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTOElectricityBill>> GetElectricityBill()
        {
            try
            {
                var query = QueryConstant.GetElectricityBill;

                using (var connection = context.CreateConnection())
                {
                    var ElectricityBill = await connection.QueryAsync<DTOElectricityBill>(query);
                    return ElectricityBill.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<DTOElectricityBill> GetElectricityBillById(long elcBill_id)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetElectricityBillById);
                parameters.Add("@ELC_TRN_ID", elcBill_id);

                using (var connection = context.CreateConnection())
                {
                    var ElectricityBill = await connection.QuerySingleAsync<DTOElectricityBill>(query, parameters);
                    return ElectricityBill;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveElectricityBill(DTOElectricityBill objElcBill)
        {
            try
            {
                var query = "sp_master";


                Int64 newBikeFuleId = await GenerateElectricityBillId(objElcBill.COM_ID);

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertElectricityBill);
                parameters.Add("@ELC_TRN_ID", newBikeFuleId);
                parameters.Add("@ELC_PRICE", objElcBill.ELC_PRICE);
                parameters.Add("@DATE", objElcBill.DATE);
                parameters.Add("@COM_ID", objElcBill.COM_ID);
                parameters.Add("@CRT_BY", objElcBill.CRT_BY);



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

        public async Task EditElectricityBill(DTOElectricityBill objElcBill, long elcBill_id)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateElectricityBill);
                parameters.Add("@ELC_TRN_ID", elcBill_id);
                parameters.Add("@ELC_PRICE", objElcBill.ELC_PRICE);
                parameters.Add("@DATE", objElcBill.DATE);



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

        public async Task DeleteElectricityBill(long elcBill_id)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteElectricityBill);
                parameters.Add("@ELC_TRN_ID", elcBill_id);



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

        private async Task<long> GenerateElectricityBillId(string comId)
        {
            string fixedPart = "204";
            string fixedPartSec = comId.ToString();
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 ELC_TRN_ID FROM MST_ELECTRICITY_BILL WHERE ELC_TRN_ID LIKE @likePattern ORDER BY ELC_TRN_ID DESC";

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

                long newElectricityBillId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newElectricityBillId;
            }
        }
    }
}

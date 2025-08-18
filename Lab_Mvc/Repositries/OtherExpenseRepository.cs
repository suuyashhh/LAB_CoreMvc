using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class OtherExpenseRepository : IOtherExpense
    {
        private readonly DapperContext context;

        public OtherExpenseRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTOOtherExpense>> GetOtherExpense(int comId)
        {
            try
            {
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetOtherExpense);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var OtherExpense = await connection.QueryAsync<DTOOtherExpense>(query, parameters);
                    return OtherExpense.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<DTOOtherExpense> GetOtherExpenseById(long otherEx_id, int comId)
        {
            try
            {
                var query = "dbo.sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetOtherExpenseById);
                parameters.Add("@OTHER_ID", otherEx_id);
                parameters.Add("@COM_ID", comId);

                using (var connection = context.CreateConnection())
                {
                    var OtherExpense = await connection.QuerySingleAsync<DTOOtherExpense>(query, parameters);
                    return OtherExpense;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DTOOtherExpense>> GetDateWiseOthMaterials(string from_date, string to_date, int comId)
        {
            try
            {
                const string query = "sp_master";
                using (var connection = context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", QueryConstant.GetDateWiseOthMaterials);
                    parameters.Add("@From_Date", from_date);
                    parameters.Add("@To_Date", to_date);
                    parameters.Add("@COM_ID", comId);

                    using (var multi = await connection.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure))
                    {
                        var casepapers = (await multi.ReadAsync<DTOOtherExpense>()).ToList();
                        return casepapers;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching case paper data", ex);
            }
        }
        public async Task SaveOtherExpense(DTOOtherExpense objOtherEx)
        {
            try
            {
                var query = "sp_master";


                Int64 newOtherExpenseId = await GenerateOtherExpenseId(objOtherEx.COM_ID);

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.InsertOtherExpense);
                parameters.Add("@OTHER_ID", newOtherExpenseId);
                parameters.Add("@OTHER_NAME", objOtherEx.OTHER_NAME);
                parameters.Add("@OTHER_PRICE", objOtherEx.OTHER_PRICE);
                parameters.Add("@DATE", objOtherEx.DATE);
                parameters.Add("@COM_ID", objOtherEx.COM_ID);
                parameters.Add("@CRT_BY", objOtherEx.CRT_BY);



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

        public async Task EditOtherExpense(DTOOtherExpense objOtherEx, long otherEx_id)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.UpdateOtherExpense);
                parameters.Add("@OTHER_ID", otherEx_id);
                parameters.Add("@OTHER_NAME", objOtherEx.OTHER_NAME);
                parameters.Add("@OTHER_PRICE", objOtherEx.OTHER_PRICE);
                parameters.Add("@DATE", objOtherEx.DATE);



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

        public async Task DeleteOtherExpense(long otherEx_id, int comId)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.DeleteOtherExpense);
                parameters.Add("@OTHER_ID", otherEx_id);
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

        private async Task<long> GenerateOtherExpenseId(string comId)
        {
            string fixedPart = "205";
            string fixedPartSec = comId.ToString();
            string likePattern = fixedPart + fixedPartSec + "%";

            string query = "SELECT TOP 1 OTHER_ID FROM MST_OTHER_EXPANCE WHERE OTHER_ID LIKE @likePattern ORDER BY OTHER_ID DESC";

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

                long newOtherExpenseId = long.Parse(fixedPart + fixedPartSec + nextNumber);
                return newOtherExpenseId;
            }
        }
    }
}

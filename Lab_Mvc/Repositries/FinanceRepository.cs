using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class FinanceRepository: IFinance
    {
        private readonly DapperContext context;

        public FinanceRepository(DapperContext context)
        {
            this.context = context;
        }
        public async Task<DTOFinance> GetFinanceById(string from_date, string to_date, int comId)
        {
            try
            {
                var query = QueryConstant.sp;
                using (var connection = context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", QueryConstant.GetFinanceById);
                    parameters.Add("@From_Date",from_date);
                    parameters.Add("@To_Date", to_date);
                    parameters.Add("@COM_ID", comId);

                    var FinanceIndexCount = await connection.QuerySingleAsync<DTOFinance>(query, parameters, commandType: CommandType.StoredProcedure);
                    return FinanceIndexCount;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

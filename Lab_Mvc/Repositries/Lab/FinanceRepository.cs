using Lab_Mvc.Interfaces.Lab;
using Models.Lab;
using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Lab
{
    public class FinanceRepository: DapperRepositoryBase, IFinance
    {
        public FinanceRepository(DapperContext context) : base(context)
        {
        }
        public async Task<DTOFinance> GetFinanceById(string from_date, string to_date, int comId)
        {
            try
            {
                var query = QueryConstant.sp;
                using (var connection = CreateConnection())
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


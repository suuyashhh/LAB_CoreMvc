using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class HomeRepository: IHome
    {
        private readonly DapperContext context;

        public HomeRepository(DapperContext context)
        {
            this.context = context;
        }
        public async Task<DTOHome> GetHomeById(string from_date, string to_date, int comId)
        {
            try
            {
                var query = QueryConstant.sp;
                using (var connection = context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", QueryConstant.GetHomeById);
                    parameters.Add("@From_Date", from_date);
                    parameters.Add("@To_Date", to_date);
                    parameters.Add("@COM_ID", comId);

                    var HomeIndexCount = await connection.QuerySingleAsync<DTOHome>(query, parameters, commandType: CommandType.StoredProcedure);
                    return HomeIndexCount;                   

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}

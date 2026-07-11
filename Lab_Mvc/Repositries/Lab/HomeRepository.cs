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
    public class HomeRepository: DapperRepositoryBase, IHome
    {
        public HomeRepository(DapperContext context) : base(context)
        {
        }
        public async Task<DTOHome> GetHomeById(string from_date, string to_date, int comId)
        {
            try
            {
                var query = QueryConstant.sp;
                using (var connection = CreateConnection())
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


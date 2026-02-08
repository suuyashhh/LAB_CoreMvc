using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class AdminRepository : IAdmin
    {
        private readonly DapperContext context;

        public AdminRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTOAdmin>> GetCompanies()
        {
            try
            {
                var query = QueryConstant.sp;

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetCompanies);

                using (var connection = context.CreateConnection())
                {
                    var admins = await connection.QueryAsync<DTOAdmin>(query, parameters, commandType: CommandType.StoredProcedure);
                    return admins.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

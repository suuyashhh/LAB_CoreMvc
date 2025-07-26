using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;

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
                var query = "sp_master";

                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.GetCompanies);

                using (var connection = context.CreateConnection())
                {
                    var admins = await connection.QueryAsync<DTOAdmin>(query, parameters);
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

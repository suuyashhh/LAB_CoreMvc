using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Models;
using System.Data;

namespace Lab_Mvc.Repositries
{
    public class LoginRepository : ILogin
    {
        private readonly DapperContext context;

        public LoginRepository(DapperContext context)
        {
            //_dbContext = dBContext;
            this.context = context;
        }


        public async Task<DTOLogin> Login(DTOLogin login)
        {
            try
            {
                var query = "sp_master";


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.Login);
                parameters.Add("@contact", login.CONTACT);
                parameters.Add("@pass", login.PASSWORD);



                using (var connection = context.CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<DTOLogin>(query, parameters, commandType: CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

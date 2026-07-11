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
    public class LoginRepository : DapperRepositoryBase, ILogin
    {
        public LoginRepository(DapperContext context) : base(context)
        {
        }


        public async Task<DTOLogin> Login(DTOLogin login)
        {
            try
            {
                var query = QueryConstant.sp;


                var parameters = new DynamicParameters();
                parameters.Add("@Action", QueryConstant.Login);
                parameters.Add("@contact", login.CONTACT);
                parameters.Add("@pass", login.PASSWORD);



                using (var connection = CreateConnection())
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


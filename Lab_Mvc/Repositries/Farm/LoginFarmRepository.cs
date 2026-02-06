using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Farm;
using Models.DairyFarm;
using Models.Farm;

namespace Lab_Mvc.Repositries.Farm
{
    public class LoginFarmRepository : ILoginFarm
    {
        private readonly DapperContext _dapperContext;

        public LoginFarmRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<DTOLoginFarm> LoginFarm(DTOLoginFarm loginFarm)
        {
            var query = @"SELECT USER_ID, USER_NAME 
              FROM FARM_USERS 
              WHERE CONTACT = @Contact AND PASSWORD = @Password";

            try
            {
                using (var con = _dapperContext.CreateConnection())
                {
                    var result = await con.QuerySingleOrDefaultAsync<DTOLoginFarm>(query, new { Contact = loginFarm.CONTACT, Password = loginFarm.PASSWORD });
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

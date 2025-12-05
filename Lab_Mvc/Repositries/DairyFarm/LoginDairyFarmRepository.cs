using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System.Diagnostics.CodeAnalysis;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class LoginDairyFarmRepository : ILoginDairyFarm
    {
        private readonly DapperContext _dapperContext;

        public LoginDairyFarmRepository(DapperContext dapperContext)
        { 
            _dapperContext = dapperContext;
        }

        public async Task<DTOLoginDairyFarm> LoginDairyFarm(DTOLoginDairyFarm loginDairyFarm)
        {
            var query = @"SELECT user_id, user_name, password, contact 
              FROM Users 
              WHERE contact = @Contact AND password = @Password";

            try
            {
                using (var con = _dapperContext.CreateConnection()) 
                {
                    var result = await con.QuerySingleOrDefaultAsync<DTOLoginDairyFarm>(query, new {Contact= loginDairyFarm.contact,Password = loginDairyFarm.password });
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

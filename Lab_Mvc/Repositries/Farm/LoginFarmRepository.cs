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

        public async Task<DTOLoginFarm> GetUserDetails(string user_id)
        {
            var query = @"SELECT * 
                  FROM FARM_USERS 
                  WHERE USER_ID = @USER_ID ";

            try
            {
                using (var con = _dapperContext.CreateConnection())
                {
                    var result = await con.QuerySingleOrDefaultAsync<DTOLoginFarm>(query, new { USER_ID = user_id });
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DTOLoginFarm> UpdatetUserDetails(DTOLoginFarm USERDETAILS)
        {
            var query = @"UPDATE FARM_USERS 
                          SET USER_NAME = @USER_NAME,
                              PASSWORD = @PASSWORD,
                              FROM_DATE = @FROM_DATE,
                              TO_DATE = @TO_DATE,
                              CONTACT = @CONTACT
                          WHERE USER_ID = @USER_ID";

            try
            {
                using (var con = _dapperContext.CreateConnection())
                {
                    // Execute the update command
                    var rowsAffected = await con.ExecuteAsync(query, new
                    {
                        USER_ID = USERDETAILS.USER_ID,
                        USER_NAME = USERDETAILS.USER_NAME,
                        PASSWORD = USERDETAILS.PASSWORD,
                        FROM_DATE = USERDETAILS.FROM_DATE,
                        TO_DATE = USERDETAILS.TO_DATE,
                        CONTACT = USERDETAILS.CONTACT
                    });

                    if (rowsAffected > 0)
                    {
                        // Fetch and return the updated user details
                        var selectQuery = @"SELECT * FROM FARM_USERS WHERE USER_ID = @USER_ID";
                        var updatedUser = await con.QuerySingleOrDefaultAsync<DTOLoginFarm>(selectQuery, new { USER_ID = USERDETAILS.USER_ID });
                        return updatedUser;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
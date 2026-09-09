using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System.Diagnostics.CodeAnalysis;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class LoginDairyFarmRepository : DapperRepositoryBase, ILoginDairyFarm
    {
        public LoginDairyFarmRepository(DapperContext dapperContext) : base(dapperContext)
        { 
        }

        public async Task<DTOLoginDairyFarm> LoginDairyFarm(DTOLoginDairyFarm loginDairyFarm)
        {
            var query = @"SELECT user_id, user_name, password, contact 
              FROM Users 
              WHERE contact = @Contact AND password = @Password";

            try
            {
                using (var con = CreateConnection()) 
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
        public async Task<bool> RegisterDairyFarm(DTOLoginDairyFarm registerDairyFarm)
        {
            var checkQuery = @"SELECT COUNT(*) FROM Users WHERE contact = @Contact";
            var insertQuery = @"INSERT INTO Users (user_name, contact, password, email, date) 
                                VALUES (@UserName, @Contact, @Password, @Email, @Date)";
            try
            {
                using (var con = CreateConnection())
                {
                    var exists = await con.ExecuteScalarAsync<int>(checkQuery, new { Contact = registerDairyFarm.contact });
                    if (exists > 0)
                        return false; // User already exists
                    
                    var rowsAffected = await con.ExecuteAsync(insertQuery, new { 
                        UserName = registerDairyFarm.user_name,
                        Contact = registerDairyFarm.contact, 
                        Password = registerDairyFarm.password,
                        Email = registerDairyFarm.email,
                        Date = DateTime.Now
                    });
                    return rowsAffected > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

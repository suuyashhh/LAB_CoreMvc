using Dapper;
using Models;
using Lab_Mvc.Contest;
using SmartParking.Interfaces;

using System.Data;

namespace SmartParking.Repositories
{
    public class ParkingLoginRepository : DapperRepositoryBase, IParkingLogin
    {
        public ParkingLoginRepository(DapperContext context) : base(context)
        {
        }


        public async Task<DTOParkingLogin> Login(string phone, string pass)
        {
            try
            {
                var query = @"SELECT USERID, NAME, EMAIL, PASS, PHONE FROM SMARTPARKING_Users WHERE PHONE = @Con AND PASS = @Pass";


                using (var con = CreateConnection())
                {
                    var result = await con.QuerySingleOrDefaultAsync<DTOParkingLogin>(query, new { Con = phone, Pass = pass });
                    return result!;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

using Dapper;
using Lab_Mvc.Contest;
using Models.SmartParking;
using System.Data;

using SmartParking.Interfaces;

namespace SmartParking.Repositories
{
    public class ParkingRegistrationRepository : DapperRepositoryBase, IParkingRegistration
    {
        public ParkingRegistrationRepository(DapperContext context) : base(context)
        {
        }

        public async Task<int> Register(DTOParkingRegistration registration)
        {
            var query = @"INSERT INTO SMARTPARKING_Users (NAME, EMAIL, PASS, PHONE) 
                          VALUES (@NAME, @EMAIL, @PASS, @PHONE)";

            using (var con = CreateConnection())
            {
                var result = await con.ExecuteAsync(query, registration);
                return result;
            }
        }
    }
}

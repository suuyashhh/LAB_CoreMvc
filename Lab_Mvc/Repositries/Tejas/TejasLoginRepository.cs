using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Tejas;
using Models.Tejas;
using SmartParking.Interfaces;
using SmartParking.Repositories;
using Dapper;
using System.Threading.Tasks;
using System;

namespace Lab_Mvc.Repositries.Tejas
{
    public class TejasLoginRepository : DapperRepositoryBase, ITejasLogin
    {
        public TejasLoginRepository(DapperContext context) : base(context)
        {
        }

        public async Task<DTOTejasLogin?> Login(DTOTejasLogin login)
        {
            var query = @"SELECT u.USER_ID, u.USER_NAME, u.CONTACT, u.USER_IMG, u.ROLE, u.TEJAS_SHOPES_ID, s.SHOP_NAME
                          FROM Tejas_USER u
                          LEFT JOIN Tejas_Shopes s ON u.TEJAS_SHOPES_ID = s.TEJAS_SHOPES_ID
                          WHERE u.CONTACT = @Contact AND u.PASS = @Pass AND (u.ACTIVE IS NULL OR u.ACTIVE = 'Y')";

            try
            {
                using (var connection = CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<DTOTejasLogin>(query, new 
                    { 
                        Contact = login.CONTACT, 
                        Pass = login.PASS 
                    });
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

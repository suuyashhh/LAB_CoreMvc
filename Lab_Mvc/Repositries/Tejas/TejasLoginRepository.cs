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
            var query = @"SELECT USER_ID, USER_NAME, CONTACT, USER_IMG ,ROLE
                          FROM SHOP_USER 
                          WHERE CONTACT = @Contact AND PASS = @Pass";

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

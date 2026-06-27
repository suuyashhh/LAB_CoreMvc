using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Shop;
using Models.Shop;
using SmartParking.Interfaces;
using SmartParking.Repositories;
using Dapper;
using System.Threading.Tasks;
using System;

namespace Lab_Mvc.Repositries.Shop
{
    public class ShopLoginRepository : DapperRepositoryBase, IShopLogin
    {
        public ShopLoginRepository(DapperContext context) : base(context)
        {
        }

        public async Task<DTOShopLogin?> Login(DTOShopLogin login)
        {
            var query = @"SELECT USER_ID, USER_NAME, CONTACT, USER_IMG 
                          FROM SHOP_USER 
                          WHERE CONTACT = @Contact AND PASS = @Pass";

            try
            {
                using (var connection = CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<DTOShopLogin>(query, new 
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

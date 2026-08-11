using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Admin;
using Models.Admin;
using SmartParking.Repositories;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Admin
{
    public class MainAdminRepository : DapperRepositoryBase, IMainAdminRepository
    {
        public MainAdminRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<MainAdmin> LoginMainAdmin(DTOMainAdminLogin loginDto)
        {
            var query = @"SELECT Id, Username, Password 
                          FROM MainAdmin 
                          WHERE Username = @Username AND Password = @Password";
            
            try
            {
                using (var con = CreateConnection())
                {
                    var result = await con.QuerySingleOrDefaultAsync<MainAdmin>(query, new { Username = loginDto.Username, Password = loginDto.Password });
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

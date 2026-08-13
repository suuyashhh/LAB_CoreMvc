using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Admin;
using Models.Admin;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<ModuleUserDto>> GetModuleUsers(string moduleName)
        {
            string query = "";
            if (moduleName == "DairyFarm")
            {
                query = "SELECT CAST(user_id AS VARCHAR) AS Id, user_name AS Username, contact AS Contact, password AS Password FROM Users";
            }
            else if (moduleName == "Farm")
            {
                query = "SELECT CAST(USER_ID AS VARCHAR) AS Id, USER_NAME AS Username, CONTACT AS Contact, PASSWORD AS Password FROM FARM_USERS";
            }
            else if (moduleName == "Shop")
            {
                query = "SELECT CAST(USER_ID AS VARCHAR) AS Id, USER_NAME AS Username, CONTACT AS Contact, PASS AS Password FROM SHOP_USER";
            }
            else if (moduleName == "Lab")
            {
                query = "SELECT CAST(useR_ID AS VARCHAR) AS Id, useR_NAME AS Username, contact AS Contact, password AS Password FROM logindetails";
            }
            else if (moduleName == "Parking")
            {
                query = "SELECT CAST(USERID AS VARCHAR) AS Id, NAME AS Username, PHONE AS Contact, PASS AS Password FROM SMARTPARKING_Users";
            }
            else if (moduleName == "Market")
            {
                query = "SELECT CAST(UserId AS VARCHAR) AS Id, Name AS Username, contact AS Contact, pass AS Password FROM Market_Users";
            }
            else if (moduleName == "Fab")
            {
                query = "SELECT CAST(admin_id AS VARCHAR) AS Id, 'Admin' AS Username, CAST(admin_id AS VARCHAR) AS Contact, password AS Password FROM Admin UNION ALL SELECT CAST(User_id AS VARCHAR) AS Id, User_name AS Username, User_contact AS Contact, User_pass AS Password FROM Fab_Users";
            }
            else if (moduleName == "Notes")
            {
                query = "SELECT CAST(id AS VARCHAR) AS Id, name AS Username, number AS Contact, password AS Password FROM Notes_Users";
            }
            else
            {
                return new List<ModuleUserDto>();
            }

            try
            {
                using (var con = CreateConnection())
                {
                    var result = await con.QueryAsync<ModuleUserDto>(query);
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

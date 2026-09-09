using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Fab;
using Models.Fab;
using SmartParking.Repositories;
using System;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Fab
{
    public class FabLoginRepository : DapperRepositoryBase, IFabLoginRepository
    {
        public FabLoginRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<DTOFabLogin?> LoginAdmin(string contact, string password)
        {
            const string query = "SELECT admin_id AS User_contact, password AS Password FROM Admin WHERE admin_id = @contact AND password = @pass";
            try
            {
                using var con = CreateConnection();
                var admin = await con.QuerySingleOrDefaultAsync<dynamic>(query, new { contact, pass = password });
                if (admin != null)
                {
                    return new DTOFabLogin
                    {
                        User_name = "Admin",
                        User_contact = admin.User_contact,
                        Type = "Admin",
                        User_id = 99999
                    };
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DTOFabLogin?> LoginHelper(string contact, string password)
        {
            const string query = "SELECT User_id, User_name, User_contact FROM Fab_Users WHERE User_contact = @contact AND User_pass = @pass";
            try
            {
                using var con = CreateConnection();
                var helper = await con.QuerySingleOrDefaultAsync<DTOFabUsers>(query, new { contact, pass = password });
                if (helper != null)
                {
                    return new DTOFabLogin
                    {
                        User_id = helper.User_id,
                        User_name = helper.User_name,
                        User_contact = helper.User_contact,
                        Type = "Helper"
                    };
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RegisterHelper(DTOFabUsers helper)
        {
            const string checkQuery = "SELECT COUNT(1) FROM Fab_Users WHERE User_contact = @User_contact";
            const string insertQuery = "INSERT INTO Fab_Users (User_name, User_contact, User_pass, User_salary) VALUES (@User_name, @User_contact, @User_pass, @User_salary)";
            try
            {
                using var con = CreateConnection();
                int count = await con.ExecuteScalarAsync<int>(checkQuery, new { helper.User_contact });
                if (count > 0)
                {
                    return false;
                }

                int rows = await con.ExecuteAsync(insertQuery, helper);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

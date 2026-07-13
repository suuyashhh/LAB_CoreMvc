using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Fab;
using Models.Fab;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Fab
{
    public class FabUsersRepository : DapperRepositoryBase, IFabUsersRepository
    {
        public FabUsersRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<DTOFabUsers>> GetAllHelpers()
        {
            const string query = "SELECT * FROM Fab_Users";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabUsers>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateHelper(DTOFabUsers helper)
        {
            const string query = "UPDATE Fab_Users SET User_name = @User_name, User_contact = @User_contact, User_pass = @User_pass, User_salary = @User_salary WHERE User_id = @User_id";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, helper);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteHelper(int userId)
        {
            const string query = "DELETE FROM Fab_Users WHERE User_id = @userId";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, new { userId });
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

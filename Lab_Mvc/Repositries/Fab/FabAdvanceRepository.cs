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
    public class FabAdvanceRepository : DapperRepositoryBase, IFabAdvanceRepository
    {
        public FabAdvanceRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<DTOFabExpanse>> GetAdvances()
        {
            const string query = @"
                SELECT FE.Exp_id, FE.date, FE.User_advance, FE.User_id, FU.User_name, FE.Exp_name 
                FROM Fab_Expanse FE 
                INNER JOIN Fab_Users FU ON FE.User_id = FU.User_id 
                ORDER BY FE.date DESC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabExpanse>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabExpanse>> GetHelperAdvanceHistory(int userId)
        {
            const string query = "SELECT User_id, User_advance, Exp_name, date FROM Fab_Expanse WHERE User_id = @userId ORDER BY date DESC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabExpanse>(query, new { userId });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

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
    public class FabTransportRepository : DapperRepositoryBase, IFabTransportRepository
    {
        public FabTransportRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<DTOFabExpanse>> GetTransportHistory(DateTime fromDate, DateTime toDate)
        {
            // Transport records are identified by User_id = 20203
            const string query = "SELECT date, Exp_id, Exp_name, Exp_price FROM Fab_Expanse WHERE User_id = 20203 AND date BETWEEN @fromDate AND @toDate ORDER BY date ASC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabExpanse>(query, new { fromDate, toDate });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

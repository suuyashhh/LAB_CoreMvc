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
    public class FabProfitRepository : DapperRepositoryBase, IFabProfitRepository
    {
        public FabProfitRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<bool> InsertProfit(DTOFabProfit profit)
        {
            const string query = "INSERT INTO Fab_Profit (Pro_name, Pro_price, date) VALUES (@Pro_name, @Pro_price, @date)";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, profit);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateProfit(DTOFabProfit profit)
        {
            const string query = "UPDATE Fab_Profit SET Pro_name = @Pro_name, Pro_price = @Pro_price, date = @date WHERE Pro_id = @Pro_id";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, profit);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteProfit(int profitId)
        {
            const string query = "DELETE FROM Fab_Profit WHERE Pro_id = @profitId";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, new { profitId });
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabProfit>> GetProfitsByDateRange(DateTime fromDate, DateTime toDate)
        {
            const string query = "SELECT * FROM Fab_Profit WHERE date BETWEEN @fromDate AND @toDate ORDER BY date ASC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabProfit>(query, new { fromDate, toDate });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

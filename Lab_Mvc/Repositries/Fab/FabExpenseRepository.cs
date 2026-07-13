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
    public class FabExpenseRepository : DapperRepositoryBase, IFabExpenseRepository
    {
        public FabExpenseRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<bool> InsertExpense(DTOFabExpanse expense)
        {
            const string query = "INSERT INTO Fab_Expanse (User_id, Exp_name, Exp_price, User_advance, date) VALUES (@User_id, @Exp_name, @Exp_price, @User_advance, @date)";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, expense);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateExpense(DTOFabExpanse expense)
        {
            const string query = "UPDATE Fab_Expanse SET User_id = @User_id, Exp_name = @Exp_name, Exp_price = @Exp_price, User_advance = @User_advance, date = @date WHERE Exp_id = @Exp_id";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, expense);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteExpense(int expenseId)
        {
            const string query = "DELETE FROM Fab_Expanse WHERE Exp_id = @expenseId";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, new { expenseId });
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabExpanse>> GetExpensesByDateRange(DateTime fromDate, DateTime toDate, bool onlyGeneral)
        {
            string query = onlyGeneral
                ? "SELECT Exp_id, Exp_name, Exp_price, date FROM Fab_Expanse WHERE User_id IS NULL AND date BETWEEN @fromDate AND @toDate ORDER BY date ASC"
                : "SELECT FE.*, FU.User_name FROM Fab_Expanse FE LEFT JOIN Fab_Users FU ON FE.User_id = FU.User_id WHERE FE.date BETWEEN @fromDate AND @toDate ORDER BY FE.date ASC";
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

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
    public class FabSalarySlipRepository : DapperRepositoryBase, IFabSalarySlipRepository
    {
        public FabSalarySlipRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<bool> SaveSalarySlip(DTOSalarySlip salarySlip)
        {
            const string query = @"
                INSERT INTO Salary_Slip 
                (user_id, From_date, TO_date, Full_day, Half_day, Off_day, Full_salary, Half_salary, Advance_salary, Full_day_Total, Half_day_total, Advance_total, Grand_total, Slip_day)
                VALUES 
                (@user_id, @From_date, @TO_date, @Full_day, @Half_day, @Off_day, @Full_salary, @Half_salary, @Advance_salary, @Full_day_Total, @Half_day_total, @Advance_total, @Grand_total, @Slip_day)";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, salarySlip);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOSalarySlip>> GetSalarySlipsHistory()
        {
            const string query = @"
                SELECT SS.*, FU.User_name 
                FROM Salary_Slip SS 
                LEFT JOIN Fab_Users FU ON SS.User_id = FU.User_id 
                ORDER BY SS.Slip_Day DESC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOSalarySlip>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOSalarySlip>> GetHelperSalaryHistory(int userId)
        {
            const string query = @"
                SELECT SS.*, FU.User_name 
                FROM Salary_Slip SS 
                LEFT JOIN Fab_Users FU ON SS.User_id = FU.User_id 
                WHERE SS.User_id = @userId 
                ORDER BY SS.Slip_Day DESC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOSalarySlip>(query, new { userId });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOSalarySlip>> GetSalarySlipsByDateRange(DateTime fromDate, DateTime toDate)
        {
            const string query = @"
                SELECT SS.*, FU.User_name 
                FROM Salary_Slip SS 
                LEFT JOIN Fab_Users FU ON SS.User_id = FU.User_id 
                WHERE SS.Slip_Day BETWEEN @fromDate AND @toDate 
                ORDER BY SS.Slip_Day ASC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOSalarySlip>(query, new { fromDate, toDate });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

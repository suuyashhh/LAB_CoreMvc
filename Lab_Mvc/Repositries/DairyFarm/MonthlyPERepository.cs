// MonthlyPERepository.cs
using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class MonthlyPERepository : IMonthlyPERepository
    {
        private readonly DapperContext _dapperContext;

        public MonthlyPERepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<MonthlyPEDto>> GetMonthlySummary(int userId)
        {
            var sql = @"
                WITH MonthlySummary AS (
                    SELECT 
                        DATENAME(MONTH, b.date) AS MonthName,
                        YEAR(b.date) AS YearValue,
                        MONTH(b.date) AS MonthNumber,
                        SUM(b.price) AS TotalBill,
                        0 AS TotalExpense
                    FROM 
                        Bill b
                    WHERE 
                        b.user_id = @userId
                    GROUP BY 
                        DATENAME(MONTH, b.date), 
                        YEAR(b.date), 
                        MONTH(b.date)
                    
                    UNION ALL
                    
                    SELECT 
                        DATENAME(MONTH, e.date) AS MonthName,
                        YEAR(e.date) AS YearValue,
                        MONTH(e.date) AS MonthNumber,
                        0 AS TotalBill,
                        SUM(e.price) AS TotalExpense
                    FROM 
                        Expense e
                    WHERE 
                        e.user_id = @userId
                    GROUP BY 
                        DATENAME(MONTH, e.date), 
                        YEAR(e.date), 
                        MONTH(e.date)
                )
                SELECT 
                    MonthName, 
                    YearValue,
                    MonthNumber,
                    SUM(TotalBill) AS TotalBill, 
                    SUM(TotalExpense) AS TotalExpense, 
                    (SUM(TotalBill) - SUM(TotalExpense)) AS Profit
                FROM 
                    MonthlySummary
                GROUP BY 
                    MonthName, YearValue, MonthNumber
                ORDER BY 
                    YearValue DESC, MonthNumber DESC";

            using var conn = _dapperContext.CreateConnection();
            return await conn.QueryAsync<MonthlyPEDto>(sql, new { userId });
        }
    }
}
// DatePERepository.cs
using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class DatePERepository : IDatePERepository
    {
        private readonly DapperContext _dapperContext;

        public DatePERepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<DatePEDto> GetDateRangeSummary(DatePEQueryDto query)
        {
            var sql = @"
                SELECT 
                    ISNULL(b.TotalBill, 0) AS TotalBill,
                    ISNULL(e.TotalExpense, 0) AS TotalExpense,
                    ISNULL(b.TotalBill, 0) - ISNULL(e.TotalExpense, 0) AS Profit
                FROM (
                    SELECT SUM(price) AS TotalBill
                    FROM Bill 
                    WHERE user_id = @UserId 
                    AND date BETWEEN @FromDate AND @ToDate
                ) b
                CROSS JOIN (
                    SELECT SUM(price) AS TotalExpense
                    FROM Expense
                    WHERE user_id = @UserId 
                    AND date BETWEEN @FromDate AND @ToDate
                ) e";

            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QuerySingleOrDefaultAsync<DatePEDto>(sql, query);

            if (result == null)
            {
                return new DatePEDto
                {
                    FromDate = query.FromDate,
                    ToDate = query.ToDate,
                    TotalBill = 0,
                    TotalExpense = 0,
                    Profit = 0
                };
            }

            result.FromDate = query.FromDate;
            result.ToDate = query.ToDate;
            return result;
        }
    }
}
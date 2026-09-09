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
    public class FabPESummaryRepository : DapperRepositoryBase, IFabPESummaryRepository
    {
        public FabPESummaryRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<DTOFabDatePE> GetProfitExpenseSummaryForDateRange(DateTime fromDate, DateTime toDate)
        {
            const string query = @"
                WITH MonthlySummary AS (
                    SELECT 
                        'Bill' AS Category,
                        SUM(CAST(fp.Pro_price AS DECIMAL(18, 2))) AS TotalAmount,
                        0 AS TotalExpense
                    FROM 
                        Fab_profit fp
                    WHERE 
                        fp.date BETWEEN @fromDate AND @toDate

                    UNION ALL

                    SELECT 
                        'Expense' AS Category,
                        0 AS TotalAmount,
                        SUM(ISNULL(CAST(fe.Exp_price AS DECIMAL(18, 2)), 0) + ISNULL(CAST(fe.user_advance AS DECIMAL(18, 2)), 0)) AS TotalExpense
                    FROM 
                        Fab_Expanse fe
                    WHERE 
                        fe.date BETWEEN @fromDate AND @toDate

                    UNION ALL

                    SELECT 
                        'Salary' AS Category,
                        0 AS TotalAmount,
                        SUM(ISNULL(CAST(ss.Grand_Total AS DECIMAL(18, 2)), 0)) AS TotalExpense
                    FROM 
                        Salary_Slip ss
                    WHERE 
                        ss.Slip_Day BETWEEN @fromDate AND @toDate
                )
                SELECT 
                    SUM(TotalAmount) AS TotalBill,
                    SUM(TotalExpense) AS TotalExpense,
                    (SUM(TotalAmount) - SUM(TotalExpense)) AS Profit
                FROM 
                    MonthlySummary;";

            try
            {
                using var con = CreateConnection();
                return await con.QuerySingleOrDefaultAsync<DTOFabDatePE>(query, new { fromDate, toDate }) 
                       ?? new DTOFabDatePE { TotalBill = 0, TotalExpense = 0, Profit = 0 };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabMonthlyPE>> GetMonthlyProfitExpenseSummary()
        {
            const string query = @"
                WITH MonthlySummary AS (
                    SELECT 
                        DATENAME(MONTH, fp.date) AS MonthName,
                        YEAR(fp.date) AS YearValue,
                        MONTH(fp.date) AS MonthNumber,
                        SUM(CAST(fp.Pro_price AS DECIMAL(18, 2))) AS TotalBill,
                        0 AS TotalExpense
                    FROM 
                        Fab_profit fp
                    GROUP BY 
                        DATENAME(MONTH, fp.date),
                        YEAR(fp.date),
                        MONTH(fp.date)
                    
                    UNION ALL

                    SELECT 
                        DATENAME(MONTH, fe.date) AS MonthName,
                        YEAR(fe.date) AS YearValue,
                        MONTH(fe.date) AS MonthNumber,
                        0 AS TotalBill,
                        SUM(CAST(ISNULL(fe.Exp_price, 0) AS DECIMAL(18, 2)) + CAST(ISNULL(fe.user_advance, 0) AS DECIMAL(18, 2))) AS TotalExpense
                    FROM 
                        Fab_Expanse fe
                    GROUP BY 
                        DATENAME(MONTH, fe.date),
                        YEAR(fe.date),
                        MONTH(fe.date)
                    
                    UNION ALL

                    SELECT 
                        DATENAME(MONTH, ss.Slip_Day) AS MonthName,
                        YEAR(ss.Slip_Day) AS YearValue,
                        MONTH(ss.Slip_Day) AS MonthNumber,
                        0 AS TotalBill,
                        SUM(CAST(ISNULL(ss.Grand_Total, 0) AS DECIMAL(18, 2))) AS TotalExpense
                    FROM 
                        Salary_Slip ss
                    GROUP BY 
                        DATENAME(MONTH, ss.Slip_Day),
                        YEAR(ss.Slip_Day),
                        MONTH(ss.Slip_Day)
                )
                SELECT 
                    MonthName,
                    YearValue,
                    SUM(TotalBill) AS TotalBill,
                    SUM(TotalExpense) AS TotalExpense,
                    (SUM(TotalBill) - SUM(TotalExpense)) AS Profit
                FROM 
                    MonthlySummary
                GROUP BY 
                    MonthName, YearValue, MonthNumber
                ORDER BY 
                    YearValue DESC, MonthNumber DESC;";

            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabMonthlyPE>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Fab;
using Models.Fab;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Fab
{
    public class FabAttendanceRepository : DapperRepositoryBase, IFabAttendanceRepository
    {
        public FabAttendanceRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<bool> CheckAttendanceExists(int userId, string userName, DateTime date)
        {
            const string query = "SELECT COUNT(1) FROM Fab_Helper_Att WHERE CAST(date AS DATE) = CAST(@date AS DATE) AND User_id = @userId";
            try
            {
                using var con = CreateConnection();
                int count = await con.ExecuteScalarAsync<int>(query, new { userId, date });
                return count > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> MarkAttendance(DTOFabHelperAtt attendance)
        {
            const string query = "INSERT INTO Fab_Helper_Att (User_id, User_name, User_day, date) VALUES (@User_id, @User_name, @User_day, @date)";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, attendance);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAttendance(DTOFabHelperAtt attendance)
        {
            const string query = "UPDATE Fab_Helper_Att SET User_day = @User_day, date = @date WHERE H_id = @H_id";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, attendance);
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAttendance(int hId)
        {
            const string query = "DELETE FROM Fab_Helper_Att WHERE H_id = @hId";
            try
            {
                using var con = CreateConnection();
                int rows = await con.ExecuteAsync(query, new { hId });
                return rows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabHelperAtt>> GetAttendanceByDate(DateTime date)
        {
            const string query = "SELECT * FROM Fab_Helper_Att WHERE CAST(date AS DATE) = CAST(@date AS DATE)";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabHelperAtt>(query, new { date });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabHelperAtt>> GetAttendanceByDateRange(DateTime fromDate, DateTime toDate)
        {
            const string query = "SELECT CAST(date AS DATE) as date, H_id, User_name, User_day, User_id FROM Fab_Helper_Att WHERE CAST(date AS DATE) BETWEEN CAST(@fromDate AS DATE) AND CAST(@toDate AS DATE) ORDER BY CAST(date AS DATE) DESC";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabHelperAtt>(query, new { fromDate, toDate });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabHelperAtt>> GetHelperAttendanceForMonth(int userId, int month, int year)
        {
            const string query = "SELECT CAST(date AS DATE) AS date, User_day, User_id, H_id, User_name FROM Fab_Helper_Att WHERE User_id = @userId AND MONTH(date) = @month AND YEAR(date) = @year";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabHelperAtt>(query, new { userId, month, year });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DTOFabAttendanceSummary?> GetHelperAttendanceSummary(int userId, DateTime fromDate, DateTime toDate)
        {
            const string query = @"
                WITH AttendanceSummary AS (
                    SELECT 
                        HA.User_id,
                        SUM(CASE WHEN HA.User_day = 'Full Day' THEN 1 ELSE 0 END) AS FullDay_Count,
                        SUM(CASE WHEN HA.User_day = 'Half Day' THEN 1 ELSE 0 END) AS HalfDay_Count,
                        SUM(CASE WHEN HA.User_day = 'Off Day' THEN 1 ELSE 0 END) AS OffDay_Count
                    FROM
                        Fab_Helper_Att HA
                    WHERE 
                        CAST(HA.date AS DATE) BETWEEN @fromDate AND @toDate
                    GROUP BY 
                        HA.User_id
                ),
                ExpenseSummary AS (
                    SELECT 
                        FE.User_id,
                        SUM(FE.User_advance) AS TOTAL_ADVANCE
                    FROM 
                        Fab_Expanse FE
                    WHERE 
                        CAST(FE.date AS DATE) BETWEEN @fromDate AND @toDate
                    GROUP BY 
                        FE.User_id
                )
                SELECT 
                    FU.User_id,
                    FU.User_name,
                    FU.User_salary,
                    COALESCE(ES.TOTAL_ADVANCE, 0) AS TOTAL_ADVANCE,
                    COALESCE(ASUM.FullDay_Count, 0) AS FullDay_Count,
                    COALESCE(ASUM.HalfDay_Count, 0) AS HalfDay_Count,
                    COALESCE(ASUM.OffDay_Count, 0) AS OffDay_Count
                FROM 
                    Fab_Users FU
                LEFT JOIN 
                    AttendanceSummary ASUM ON FU.User_id = ASUM.User_id
                LEFT JOIN 
                    ExpenseSummary ES ON FU.User_id = ES.User_id
                WHERE 
                    FU.User_id = @userId";

            try
            {
                using var con = CreateConnection();
                return await con.QuerySingleOrDefaultAsync<DTOFabAttendanceSummary>(query, new { userId, fromDate, toDate });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AutoMarkOffDaysMissingAttendance(DateTime date)
        {
            const string missingQuery = @"
                SELECT u.User_id, u.User_name
                FROM Fab_Users u
                LEFT JOIN Fab_Helper_Att a ON u.User_id = a.User_id AND CAST(a.date AS DATE) = CAST(@date AS DATE)
                WHERE a.User_id IS NULL";

            const string insertQuery = @"
                INSERT INTO Fab_Helper_Att (User_id, User_name, User_day, date)
                VALUES (@User_id, @User_name, 'Off Day', @date)";

            try
            {
                using var con = CreateConnection();
                var missing = (await con.QueryAsync<DTOFabUsers>(missingQuery, new { date })).ToList();
                int count = 0;
                foreach (var user in missing)
                {
                    int rows = await con.ExecuteAsync(insertQuery, new { User_id = user.User_id, User_name = user.User_name, date });
                    if (rows > 0) count++;
                }
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DTOFabHelperAtt>> GetHelperAttendanceByRange(int userId, DateTime fromDate, DateTime toDate)
        {
            const string query = @"
                SELECT CAST(date AS DATE) AS date, H_id, User_name, User_day, User_id 
                FROM Fab_Helper_Att 
                WHERE User_id = @userId AND CAST(date AS DATE) BETWEEN CAST(@fromDate AS DATE) AND CAST(@toDate AS DATE) 
                ORDER BY date";
            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabHelperAtt>(query, new { userId, fromDate, toDate });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

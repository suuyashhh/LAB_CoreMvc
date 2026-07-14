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
    public class FabHistoryRepository : DapperRepositoryBase, IFabHistoryRepository
    {
        public FabHistoryRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<DTOFabHistory>> GetAllHistory()
        {
            const string query = @"
                SELECT 
                    Exp_id AS record_id,
                    User_id,
                    CASE 
                        WHEN User_id IS NULL THEN 'GoodExpanse'
                        WHEN User_id = 20203 THEN 'TransportPlace'
                        WHEN User_id BETWEEN 0 AND 1000 THEN 'Advance'
                        ELSE 'Other'
                    END AS record_type,
                    CASE 
                        WHEN User_id BETWEEN 0 AND 1000 THEN 
                            (SELECT user_name 
                             FROM Fab_Users 
                             WHERE Fab_Users.user_id = Fab_Expanse.User_id)
                        ELSE Exp_name
                    END AS record_name,
                    CASE
                        WHEN User_id BETWEEN 0 AND 1000 THEN User_advance
                        ELSE Exp_price
                    END AS price,
                    date AS record_date
                FROM 
                    Fab_Expanse

                UNION ALL

                SELECT 
                    Slip_id AS record_id,
                    User_id,
                    'SalarySlip' AS record_type,
                    (SELECT user_name 
                     FROM Fab_Users 
                     WHERE Fab_Users.user_id = Salary_Slip.User_id) AS record_name,
                    Grand_Total AS price,
                    Slip_Day AS record_date
                FROM Salary_Slip

                UNION ALL

                SELECT 
                    Pro_id AS record_id,
                    NULL AS User_id,
                    'FabProfit' AS record_type,
                    Pro_name AS record_name,
                    Pro_price AS price,
                    date AS record_date
                FROM dbo.Fab_Profit

                ORDER BY record_date DESC, User_id;
            ";

            try
            {
                using var con = CreateConnection();
                return await con.QueryAsync<DTOFabHistory>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

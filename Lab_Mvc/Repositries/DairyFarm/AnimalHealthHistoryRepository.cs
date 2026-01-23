using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class AnimalHealthHistoryRepository : IAnimalHealthHistory
    {
        private readonly DapperContext context;

        public AnimalHealthHistoryRepository(DapperContext context)
        {
            this.context = context;
        }

        // Get all animals with health record summary
        public async Task<IEnumerable<AnimalHealthSummaryDTO>> GetAllAnimalsWithHealthSummary(int userId)
        {
            var query = @"
                SELECT 
                    an.animal_id AS AnimalId,
                    an.animal_name AS AnimalName,
                    COUNT(e.expense_id) AS TotalHealthRecords,
                    ISNULL(SUM(e.price), 0) AS TotalExpenses
                FROM AnimalsName an
                LEFT JOIN Expense e ON e.user_id = an.user_id 
                    AND e.Animal_id = an.animal_id 
                    AND e.expense_name IN ('Medicine', 'Doctor')
                WHERE an.user_id = @UserId
                GROUP BY an.animal_id, an.animal_name
                ORDER BY an.animal_name";

            using (var connection = context.CreateConnection())
            {
                var animals = await connection.QueryAsync<AnimalHealthSummaryDTO>(query, new { UserId = userId });
                return animals.ToList();
            }
        }

        // Get health history for specific animal
        public async Task<IEnumerable<AnimalHealthRecordDTO>> GetAnimalHealthHistory(int userId, int animalId)
        {
            var query = @"
                SELECT 
                    e.expense_id AS RecordId,
                    e.user_id AS UserId,
                    e.Animal_id AS AnimalId,
                    an.animal_name AS AnimalName,
                    e.expense_name AS RecordType,
                    e.reason AS Reason,
                    e.price AS Price,
                    e.date AS RecordDate,
                    FORMAT(e.date, 'dd-MMM-yyyy') AS FormattedDate,
                    FORMAT(e.date, 'MMMM yyyy') AS MonthYear
                FROM Expense e
                INNER JOIN AnimalsName an ON an.animal_id = e.Animal_id AND an.user_id = e.user_id
                WHERE e.user_id = @UserId 
                    AND e.Animal_id = @AnimalId
                    AND e.expense_name IN ('Medicine', 'Doctor')
                ORDER BY e.date DESC";

            using (var connection = context.CreateConnection())
            {
                var history = await connection.QueryAsync<AnimalHealthRecordDTO>(query,
                    new { UserId = userId, AnimalId = animalId });
                return history.ToList();
            }
        }

        public async Task<IEnumerable<AnimalHealthSummaryDTO>> GetAllAnimalsWithImage(int userId)
        {
            var query = @"
  SELECT 
    an.animal_id AS AnimalId,
    an.animal_name AS AnimalName,
    COUNT(e.expense_id) AS TotalHealthRecords,
    ISNULL(SUM(e.price), 0) AS TotalExpenses,
    an.animal_image AS AnimalImage
FROM AnimalsName an
LEFT JOIN Expense e 
    ON e.user_id = an.user_id 
   AND e.Animal_id = an.animal_id 
   AND e.expense_name IN ('Medicine', 'Doctor')
WHERE an.user_id = @UserId
GROUP BY 
    an.animal_id, 
    an.animal_name,
    an.animal_image
ORDER BY an.animal_name";

            using (var connection = context.CreateConnection())
            {
                var animals = await connection.QueryAsync<AnimalHealthSummaryDTO>(query, new { UserId = userId });
                return animals.ToList();
            }
        }
    }
}
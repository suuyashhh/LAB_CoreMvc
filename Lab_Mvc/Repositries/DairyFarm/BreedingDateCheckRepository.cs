using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class BreedingDateCheckRepository : IBreedingDateCheck
    {
        private readonly DapperContext context;

        public BreedingDateCheckRepository(DapperContext context)
        {
            this.context = context;
        }

        // Get all animals with breeding record summary
        public async Task<IEnumerable<BreedingAnimalSummaryDTO>> GetAllAnimalsWithBreedingSummary(int userId)
        {
            var query = @"               
                SELECT 
    an.animal_id AS AnimalId,
    an.animal_name AS AnimalName,
	an.animal_image AS AnimalImage,
    COUNT(e.expense_id) AS TotalBreedingRecords,
    MAX(e.date) AS LastBreedingDate,
    (
        SELECT TOP 1 e2.reason
        FROM Expense e2
        WHERE e2.user_id = an.user_id
          AND e2.Animal_id = an.animal_id
          AND e2.Switch = 22
        ORDER BY e2.date DESC
    ) AS LastBreedingReason
FROM AnimalsName an
LEFT JOIN Expense e 
       ON e.user_id = an.user_id 
      AND e.Animal_id = an.animal_id 
      AND e.Switch = 22
WHERE an.user_id = @UserId
GROUP BY 
    an.user_id,          -- ✅ ADD THIS
    an.animal_id, 
    an.animal_image,
    an.animal_name
ORDER BY an.animal_name;";

            try
            {

                using (var connection = context.CreateConnection())
                {
                    var animals = await connection.QueryAsync<BreedingAnimalSummaryDTO>(query, new { UserId = userId });

                    // Process each animal to add calculated fields
                    foreach (var animal in animals)
                    {
                        CalculateBreedingStatus(animal);
                    }

                    return animals.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get breeding history for specific animal
        public async Task<IEnumerable<BreedingRecordDTO>> GetAnimalBreedingHistory(int userId, int animalId)
        {
            var query = @"
                SELECT 
                    e.expense_id AS RecordId,
                    e.user_id AS UserId,
                    e.Animal_id AS AnimalId,
                    an.animal_name AS AnimalName,
                    e.reason AS Reason,
                    e.date AS BreedingDate,
                    FORMAT(e.date, 'dd-MMM-yyyy') AS FormattedDate,
                    FORMAT(e.date, 'MMMM yyyy') AS MonthYear,
                    DATEDIFF(day, e.date, GETDATE()) AS DaysSinceBreeding
                FROM Expense e
                INNER JOIN AnimalsName an ON an.animal_id = e.Animal_id AND an.user_id = e.user_id
                WHERE e.user_id = @UserId 
                    AND e.Animal_id = @AnimalId
                    AND e.Switch = 22
                ORDER BY e.date DESC";

            using (var connection = context.CreateConnection())
            {
                var records = await connection.QueryAsync<BreedingRecordDTO>(query,
                    new { UserId = userId, AnimalId = animalId });

                // Add status to each record
                foreach (var record in records)
                {
                    record.Status = GetBreedingStatus(record.DaysSinceBreeding);
                }

                return records.ToList();
            }
        }

        // Helper method to calculate breeding status for animal summary
        private void CalculateBreedingStatus(BreedingAnimalSummaryDTO animal)
        {
            if (animal.LastBreedingDate.HasValue)
            {
                var daysSinceBreeding = (DateTime.Now - animal.LastBreedingDate.Value).Days;
                animal.Status = GetBreedingStatus(daysSinceBreeding);
            }
            else
            {
                animal.Status = "No Records";
            }
        }

        // Helper method to determine breeding status
        private string GetBreedingStatus(int daysSinceBreeding)
        {
            if (daysSinceBreeding <= 30)
                return "Recent";
            else if (daysSinceBreeding <= 90)
                return "Ongoing";
            else
                return "Overdue";
        }
    }
}
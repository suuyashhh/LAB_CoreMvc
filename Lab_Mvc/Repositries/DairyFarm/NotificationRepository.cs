using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;

public class NotificationRepository : INotification
{
    private readonly DapperContext _dapperContext;
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(DapperContext dapperContext, ILogger<NotificationRepository> logger)
    {
        _dapperContext = dapperContext;
        _logger = logger;
    }

    public async Task<List<DTONotification>> GetBreedingNotifications(int userId)
    {
        using var conn = _dapperContext.CreateConnection();

        try
        {
            _logger.LogDebug("GetBreedingNotifications called for userId: {UserId}", userId);

            // Start background update
            _ = Task.Run(() => UpdateBreedingNotifications(userId));

            // Immediately return existing notifications
            var query = @"
SELECT 
    n.Id,
    n.AnimalId,
    a.animal_name AS AnimalName,
    n.LastBreedingDate,
    n.MonthNo
FROM BreedingNotifications n
JOIN AnimalsName a ON a.animal_id = n.AnimalId
WHERE n.UserId = @UserId 
    AND n.IsChecked = 0
    AND n.UpdatedOn >= DATEADD(HOUR, -1, GETDATE())
ORDER BY n.CreatedOn DESC";

            _logger.LogDebug("Executing query: {Query}", query);

            var list = await conn.QueryAsync<DTONotification>(query, new { UserId = userId });

            // DEBUG: Log raw results from database
            _logger.LogDebug("Raw database results count: {Count}", list?.Count() ?? 0);

            foreach (var item in list)
            {
                _logger.LogDebug("DB Record -> Id: {Id}, AnimalId: {AnimalId}, AnimalName: {AnimalName}, " +
                               "LastBreedingDate: {LastBreedingDate}, MonthNo: {MonthNo}",
                               item.Id, item.AnimalId, item.AnimalName, item.LastBreedingDate, item.MonthNo);
            }

            var resultList = list.ToList();

            foreach (var n in resultList)
            {
                n.Message = $"🐄 {n.AnimalName} — Breeding ला {n.MonthNo} महिने पूर्ण झाले आहेत.";
                _logger.LogDebug("Added message for Id {Id}: {Message}", n.Id, n.Message);
            }

            // DEBUG: Log final result
            _logger.LogDebug("Returning {Count} notifications for userId {UserId}",
                           resultList.Count, userId);

            return resultList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting breeding notifications for user {UserId}", userId);

            // DEBUG: Show what we're returning on error
            _logger.LogDebug("Returning empty list due to error for userId {UserId}", userId);

            return new List<DTONotification>();
        }
    }

    private async Task UpdateBreedingNotifications(int userId)
    {
        try
        {
            _logger.LogDebug("UpdateBreedingNotifications started for userId: {UserId}", userId);

            using var conn = _dapperContext.CreateConnection();

            var query = @"
SELECT 
    an.animal_id AS AnimalId,
    an.animal_name AS AnimalName,
    MAX(e.date) AS LastBreedingDate
FROM AnimalsName an
LEFT JOIN Expense e 
    ON e.Animal_id = an.animal_id 
   AND e.Switch = 22
WHERE an.user_id = @UserId
GROUP BY an.animal_id, an.animal_name";

            var animals = await conn.QueryAsync<AnimalBreedingInfo>(query, new { UserId = userId });

            int totalAnimalsProcessed = 0;
            int totalNotificationsProcessed = 0;

            foreach (var animal in animals)
            {
                totalAnimalsProcessed++;

                // ✅ NEW: If no breeding date → delete all notifications for this animal
                if (animal.LastBreedingDate == null)
                {
                    await conn.ExecuteAsync(@"
DELETE FROM BreedingNotifications
WHERE UserId = @UserId
  AND AnimalId = @AnimalId",
                        new
                        {
                            UserId = userId,
                            AnimalId = animal.AnimalId
                        });

                    _logger.LogDebug("Deleted notifications for animal {AnimalId} due to NULL breeding date",
                                     animal.AnimalId);

                    continue; // skip further processing
                }

                // ✅ Calculate months passed
                int monthsPassed = await conn.ExecuteScalarAsync<int>(@"
SELECT DATEDIFF(MONTH, @LastBreedingDate, GETDATE()) -
       CASE WHEN DAY(GETDATE()) < DAY(@LastBreedingDate) THEN 1 ELSE 0 END",
                    new { animal.LastBreedingDate });

                _logger.LogDebug("Animal {AnimalId}: {MonthsPassed} months passed",
                                 animal.AnimalId, monthsPassed);

                // ✅ Remove old records with different breeding date
                await conn.ExecuteAsync(@"
DELETE FROM BreedingNotifications
WHERE UserId = @UserId
  AND AnimalId = @AnimalId
  AND LastBreedingDate <> @LastBreedingDate",
                    new
                    {
                        UserId = userId,
                        AnimalId = animal.AnimalId,
                        LastBreedingDate = animal.LastBreedingDate
                    });

                // ✅ Insert / Update months (max 9)
                int maxMonth = Math.Min(monthsPassed, 9);

                for (int month = 1; month <= maxMonth; month++)
                {
                    var exists = await conn.ExecuteScalarAsync<int>(@"
SELECT COUNT(*)
FROM BreedingNotifications
WHERE UserId = @UserId
  AND AnimalId = @AnimalId
  AND MonthNo = @MonthNo
  AND LastBreedingDate = @LastBreedingDate",
                        new
                        {
                            UserId = userId,
                            AnimalId = animal.AnimalId,
                            MonthNo = month,
                            LastBreedingDate = animal.LastBreedingDate
                        });

                    if (exists == 0)
                    {
                        await conn.ExecuteAsync(@"
INSERT INTO BreedingNotifications
(UserId, AnimalId, LastBreedingDate, MonthNo, IsChecked, CreatedOn, UpdatedOn)
VALUES
(@UserId, @AnimalId, @LastBreedingDate, @MonthNo, 0, GETDATE(), GETDATE())",
                            new
                            {
                                UserId = userId,
                                AnimalId = animal.AnimalId,
                                LastBreedingDate = animal.LastBreedingDate,
                                MonthNo = month
                            });

                        totalNotificationsProcessed++;
                    }
                    else
                    {
                        await conn.ExecuteAsync(@"
UPDATE BreedingNotifications
SET UpdatedOn = GETDATE()
WHERE UserId = @UserId
  AND AnimalId = @AnimalId
  AND MonthNo = @MonthNo
  AND LastBreedingDate = @LastBreedingDate",
                            new
                            {
                                UserId = userId,
                                AnimalId = animal.AnimalId,
                                MonthNo = month,
                                LastBreedingDate = animal.LastBreedingDate
                            });
                    }
                }

                // ✅ Delete extra months if breeding date changed
                await conn.ExecuteAsync(@"
DELETE FROM BreedingNotifications
WHERE UserId = @UserId
  AND AnimalId = @AnimalId
  AND MonthNo > @MaxMonth",
                    new
                    {
                        UserId = userId,
                        AnimalId = animal.AnimalId,
                        MaxMonth = maxMonth
                    });
            }

            _logger.LogDebug("UpdateBreedingNotifications completed. Animals: {Animals}, Notifications: {Notifications}",
                             totalAnimalsProcessed, totalNotificationsProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating breeding notifications for user {UserId}", userId);
        }
    }


    public async Task<int> GetNotificationCount(int userId)
    {
        using var conn = _dapperContext.CreateConnection();

        try
        {
            _logger.LogDebug("GetNotificationCount called for userId: {UserId}", userId);

            var query = @"
SELECT COUNT(*) 
FROM BreedingNotifications 
WHERE UserId = @UserId 
    AND IsChecked = 0
    AND UpdatedOn >= DATEADD(HOUR, -1, GETDATE())";

            var count = await conn.ExecuteScalarAsync<int>(query, new { UserId = userId });

            // DEBUG: Log the count
            _logger.LogDebug("Notification count for userId {UserId}: {Count}", userId, count);

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification count for user {UserId}", userId);

            // DEBUG: Return 0 on error
            _logger.LogDebug("Returning 0 due to error for userId {UserId}", userId);

            return 0;
        }
    }

    public async Task MarkAsChecked(int id)
    {
        using var conn = _dapperContext.CreateConnection();

        try
        {
            _logger.LogDebug("MarkAsChecked called for notification Id: {Id}", id);

            // First, check if notification exists
            var checkQuery = "SELECT COUNT(*) FROM BreedingNotifications WHERE Id = @Id";
            var exists = await conn.ExecuteScalarAsync<int>(checkQuery, new { Id = id });

            if (exists == 0)
            {
                _logger.LogWarning("Notification with Id {Id} not found", id);
                return;
            }

            var updateQuery = @"
UPDATE BreedingNotifications 
SET IsChecked = 1, 
    UpdatedOn = GETDATE()
WHERE Id = @Id";

            var rowsAffected = await conn.ExecuteAsync(updateQuery, new { Id = id });

            // DEBUG: Log the result
            _logger.LogDebug("MarkAsChecked: {RowsAffected} row(s) affected for Id {Id}",
                           rowsAffected, id);

            if (rowsAffected > 0)
            {
                _logger.LogDebug("Successfully marked notification Id {Id} as checked", id);
            }
            else
            {
                _logger.LogWarning("Failed to mark notification Id {Id} as checked", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {Id} as checked", id);
            throw; // Re-throw to let controller handle it
        }
    }

    // Helper class for internal use
    private class AnimalBreedingInfo
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; }
        public DateTime? LastBreedingDate { get; set; }
    }

}
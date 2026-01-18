using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models.DairyFarm;
using System.Data;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class HistoryDairyRepository : IHistoryDairy
    {
        private readonly DapperContext context;

        public HistoryDairyRepository(DapperContext context)
        {
            this.context = context;
        }

        // ================= GET ALL HISTORY =================
        public async Task<IEnumerable<HistoryDTO>> GetAllHistory(int userId)
        {
            // UNION query to get all expenses from different tables/sources
            var query = @"
                -- Feeds History
                SELECT 
                    expense_id,
                    user_id,
                    expense_name,
                    feed_name,
                    price,
                    quantity,
                    NULL as reason,
                    NULL as animal_name,
                    date,
                    NULL as img,
                    NULL as animal_type
                FROM Expense 
                WHERE user_id = @UserId AND expense_name = 'Feeds'
                
                UNION ALL
                
                -- OtherFeeds History
                SELECT 
                    expense_id,
                    user_id,
                    expense_name,
                    feed_name,
                    price,
                    quantity,
                    NULL as reason,
                    NULL as animal_name,
                    date,
                    img,
                    NULL as animal_type
                FROM Expense 
                WHERE user_id = @UserId AND expense_name = 'OtherFeeds'
                
                UNION ALL
                
                -- Medicine History
                SELECT 
                    expense_id,
                    user_id,
                    expense_name,
                    NULL as feed_name,
                    price,
                    NULL as quantity,
                    reason,
                    animal_name,
                    date,
                    img as img,
                    NULL as animal_type
                FROM Expense 
                WHERE user_id = @UserId AND expense_name = 'Medicine'
                
                UNION ALL
                
                -- Doctor History
                SELECT 
                    expense_id,
                    user_id,
                    expense_name,
                    NULL as feed_name,
                    price,
                    NULL as quantity,
                    reason,
                    animal_name,
                    date,
                    NULL as img,
                    NULL as animal_type
                FROM Expense 
                WHERE user_id = @UserId AND expense_name = 'Doctor'
                
                UNION ALL
                
                -- Bill History
                SELECT 
                    bill_id as expense_id,
                    user_id,
                    'Bill' as expense_name,
                    NULL as feed_name,
                    price,
                    NULL as quantity,
                    NULL as reason,
                    NULL as animal_name,
                    date,
                    img,
                    animal_type
                FROM Bill 
                WHERE user_id = @UserId
                
                ORDER BY date DESC
            ";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    var history = await connection.QueryAsync<HistoryDTO>(query, new { UserId = userId });

                    // Add display properties
                    foreach (var item in history)
                    {
                        SetDisplayProperties(item);
                    }

                    return history.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        // ================= GET IMAGE BY ID =================
        public async Task<object> GetHistoryImageById(long expenseId, string expenseType)
        {
            string query = expenseType switch
            {
                "Feeds" => @"
                    SELECT f.feed_image AS Image 
                    FROM Expense AS e 
                    JOIN Feeds AS f ON f.user_id = e.user_id AND f.feed_name = e.feed_name 
                    WHERE e.expense_id = @ExpenseId",

                "OtherFeeds" => @"
                    SELECT img AS Image 
                    FROM Expense 
                    WHERE expense_id = @ExpenseId",

                "Medicine" => @"
                    SELECT img AS Image 
                    FROM Expense 
                    WHERE expense_id = @ExpenseId",

                "Doctor" => @"
                    SELECT a.animal_image AS Image 
                    FROM Expense AS e 
                    JOIN AnimalsName AS a ON a.user_id = e.user_id AND a.animal_id = e.Animal_id 
                    WHERE e.expense_id = @ExpenseId",

                "Bill" => @"
                    SELECT img AS Image 
                    FROM Bill 
                    WHERE bill_id = @ExpenseId",

                _ => @"
                    SELECT NULL AS Image"
            };

            using (var connection = context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<dynamic>(query, new { ExpenseId = expenseId });
                return result ?? new { Image = (string?)null };
            }
        }

        // ================= HELPER METHOD =================
        private void SetDisplayProperties(HistoryDTO item)
        {
            switch (item.expense_name)
            {
                case "Feeds":
                    item.DisplayTitle = item.feed_name ?? "Feed";
                    item.DisplaySubtitle = $"Quantity: {item.quantity} | Price: ₹{item.price}";
                    item.IconClass = "ri-box-3-line";
                    item.BadgeColor = "bg-green-100 text-green-800 border-green-200";
                    break;

                case "OtherFeeds":
                    item.DisplayTitle = item.feed_name ?? "Other Feed";
                    item.DisplaySubtitle = $"Quantity: {item.quantity} | Price: ₹{item.price}";
                    item.IconClass = "ri-box-3-line";
                    item.BadgeColor = "bg-blue-100 text-blue-800 border-blue-200";
                    break;

                case "Medicine":
                    item.DisplayTitle = item.reason ?? "Medicine";
                    item.DisplaySubtitle = $"Animal: {item.animal_name} | Price: ₹{item.price}";
                    item.IconClass = "ri-medicine-bottle-line";
                    item.BadgeColor = "bg-purple-100 text-purple-800 border-purple-200";
                    break;

                case "Doctor":
                    item.DisplayTitle = item.reason ?? "Doctor Visit";
                    item.DisplaySubtitle = $"Animal: {item.animal_name} | Price: ₹{item.price}";
                    item.IconClass = "ri-stethoscope-line";
                    item.BadgeColor = "bg-red-100 text-red-800 border-red-200";
                    break;

                case "Bill":
                    item.DisplayTitle = item.animal_type ?? "Bill";
                    item.DisplaySubtitle = $"Price: ₹{item.price}";
                    item.IconClass = "ri-bill-line";
                    item.BadgeColor = "bg-yellow-100 text-yellow-800 border-yellow-200";
                    break;

                default:
                    item.DisplayTitle = item.expense_name;
                    item.DisplaySubtitle = $"Price: ₹{item.price}";
                    item.IconClass = "ri-file-list-line";
                    item.BadgeColor = "bg-gray-100 text-gray-800 border-gray-200";
                    break;
            }
        }
    }
}
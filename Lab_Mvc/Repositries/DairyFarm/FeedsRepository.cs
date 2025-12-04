using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.DairyFarm;
using Models;
using Models.DairyFarm;
using System.Data;
using System.Security.AccessControl;

namespace Lab_Mvc.Repositries.DairyFarm
{
    public class FeedsRepository : IFeeds
    {
        private readonly DapperContext context;

        public FeedsRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DTOFeeds>> GetAllFeedHistory(int userId)
        {

            var query = @"
                          select * from Expense where expense_name='Feeds' AND user_id=@UserId order by date desc
                          ";
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var FeedsHistorys = await connection.QueryAsync<DTOFeeds>(query, new { UserId = userId });

                    return FeedsHistorys.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<DTOFeeds> GetFeedHistoryById(long exp_id)
        {
            var query = @"
                          select * from Expense where expense_id=@Exp_Id
                          ";
            try
            {
                using (var connection = context.CreateConnection())
                {
                    var Feed = await connection.QuerySingleAsync<DTOFeeds>(query, new { Exp_Id = exp_id }
                    );

                    return Feed;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task Save(DTOFeeds objFeed)
        {
            var query = @"
        INSERT INTO Expense (user_id, expense_name, feed_name, price, quantity, date) 
        VALUES (@UserId, @ExpName, @FeedName, @Price, @Qty, @Dt)";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        UserId = objFeed.user_id,
                        ExpName = objFeed.expense_name,
                        FeedName = objFeed.feed_name,
                        Price = objFeed.price,
                        Qty = objFeed.quantity,
                        Dt = objFeed.date
                    });
                }
            }
            catch
            {
                throw;
            }
        }


        public async Task Edit(DTOFeeds objFeed)
        {
            var query = @"
                            UPDATE Expense 
                            SET 
                                expense_name = @ExpName,
                                feed_name = @FeedName,
                                price = @Price,
                                quantity = @Qty,
                                date = @Dt
                            WHERE expense_id = @Id AND user_id = @UserId";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        Id = objFeed.expense_id,
                        UserId = objFeed.user_id,
                        ExpName = objFeed.expense_name,
                        FeedName = objFeed.feed_name,
                        Price = objFeed.price,
                        Qty = objFeed.quantity,
                        Dt = objFeed.date
                    });
                }
            }
            catch
            {
                throw;
            }
        }


        public async Task Delete(long exp_id)
        {
            var query = @"
    DELETE FROM Expense 
    WHERE expense_id = @Id";

            try
            {
                using (var connection = context.CreateConnection())
                {
                    await connection.ExecuteAsync(query, new
                    {
                        Id = exp_id
                    });
                }
            }
            catch
            {
                throw;
            }
        }



    }
}

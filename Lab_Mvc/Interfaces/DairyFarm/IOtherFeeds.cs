using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IOtherFeeds
    {
        Task<IEnumerable<DTOFeeds>> GetAllFeedHistory(int userId);
        Task<object> GetFeedImageById(long exp_id);
        Task Save(DTOFeeds objFeed);
        Task Edit(DTOFeeds objFeed);
        Task Delete(long exp_id);
    }
}

using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IHistoryDairy
    {
        Task<IEnumerable<HistoryDTO>> GetAllHistory(int userId);
        Task<object> GetHistoryImageById(long expenseId, string expenseType);
    }
}
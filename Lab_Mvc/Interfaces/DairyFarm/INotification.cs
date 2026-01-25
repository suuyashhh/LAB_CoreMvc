using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface INotification
    {
        Task<List<DTONotification>> GetBreedingNotifications(int userId);
        Task<int> GetNotificationCount(int userId);
        Task MarkAsChecked(int id);
        Task<List<int>> GetAllUserIds();
        Task RunBreedingUpdateOnly(int userId);

    }


}

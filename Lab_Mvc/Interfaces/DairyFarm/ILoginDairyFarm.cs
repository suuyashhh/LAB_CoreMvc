using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface ILoginDairyFarm
    {
        Task<DTOLoginDairyFarm> LoginDairyFarm(DTOLoginDairyFarm loginDairyFarm);
    }
}

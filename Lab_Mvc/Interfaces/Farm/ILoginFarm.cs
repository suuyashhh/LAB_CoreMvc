using Models.DairyFarm;
using Models.Farm;

namespace Lab_Mvc.Interfaces.Farm
{
    public interface ILoginFarm
    {
        Task<DTOLoginFarm> LoginFarm(DTOLoginFarm loginDairyFarm);
    }
}

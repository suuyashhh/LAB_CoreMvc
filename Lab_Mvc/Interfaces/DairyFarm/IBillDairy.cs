using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IBillDairy
    {
        Task<IEnumerable<DTOBillDairy>> GetAllBillHistory(int userId);
        Task<object> GetBillImageById(long exp_id);
        Task Save(DTOBillDairy objFeed);
        Task Edit(DTOBillDairy objFeed);
        Task Delete(long exp_id);
    }
}

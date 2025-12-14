using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IDoctorDairy
    {
        Task<IEnumerable<DTODoctorDairy>> GetAllDoctorHistory(int userId);
        Task<object> GetDocImageById(long exp_id);
        Task Save(DTODoctorDairy objDDoc);
        Task Edit(DTODoctorDairy objDDoc);
        Task Delete(long exp_id);
    }
}

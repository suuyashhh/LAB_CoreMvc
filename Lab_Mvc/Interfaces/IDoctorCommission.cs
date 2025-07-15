using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IDoctorCommission
    {
        Task<IEnumerable<DTODoctorCommission>> GetDoctorCommission(int comId);
        Task<DTODoctorCommission> GetDoctorCommissionById(long docCom_id);
        Task SaveDoctorCommission(DTODoctorCommission objDocCom);
        Task EditDoctorCommission(DTODoctorCommission objDocCom, long docCom_id);
        Task DeleteDoctorCommission(long docCom_id);
    }
}

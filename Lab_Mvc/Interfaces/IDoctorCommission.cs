using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IDoctorCommission
    {
        Task<IEnumerable<DTODoctorCommission>> GetDoctorCommission(int comId);
        Task<DTODoctorCommission> GetDoctorCommissionById(long docCom_id, int comId);
        Task<List<DTODoctorCommission>> GetDateWiseDocCommission(string from_date, string to_date, int comId);
        Task SaveDoctorCommission(DTODoctorCommission objDocCom);
        Task EditDoctorCommission(DTODoctorCommission objDocCom, long docCom_id);
        Task DeleteDoctorCommission(long docCom_id, int comId);
    }
}

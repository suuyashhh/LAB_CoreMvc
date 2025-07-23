using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IElectricityBill
    {
        Task<IEnumerable<DTOElectricityBill>> GetElectricityBill(int comId);
        Task<DTOElectricityBill> GetElectricityBillById(long elcBill_id);
        Task<List<DTOElectricityBill>> GetDateWiseElcBill(string from_date, string to_date);
        Task SaveElectricityBill(DTOElectricityBill objElcBill);
        Task EditElectricityBill(DTOElectricityBill objElcBill, long elcBill_id);
        Task DeleteElectricityBill(long elcBill_id);
    }
}

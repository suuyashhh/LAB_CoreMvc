using Models.Tejas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Tejas
{
    public interface ITejasBilling
    {
        Task<IEnumerable<TejasBill>> GetAllBills();
        Task<IEnumerable<TejasBill>> GetBillsByDateRange(System.DateTime startDate, System.DateTime endDate);
        Task<TejasBill?> GetBillById(string id);
        Task<string> InsertBill(TejasBill bill);
        Task<int> UpdateBill(TejasBill bill);
        Task<int> DeleteBill(string id);
        Task<string> GetNextBillNumber();
    }
}

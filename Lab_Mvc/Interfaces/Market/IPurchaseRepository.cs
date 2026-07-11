using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Market;

namespace Lab_Mvc.Interfaces.Market
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<PurchaseEntry>> GetAllAsync(System.DateTime? fromDate = null, System.DateTime? toDate = null);
        Task<PurchaseEntry?> GetByIdAsync(int id);
        Task<int> AddAsync(PurchaseEntry entry);
        Task<bool> UpdateAsync(PurchaseEntry entry);
        Task<bool> DeleteAsync(int id);
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<bool> UpdatePdfAsync(int id, byte[] pdfData);
        Task<byte[]?> GetPdfAsync(int id);
    }
}

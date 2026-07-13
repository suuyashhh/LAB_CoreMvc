using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabProfitRepository
    {
        Task<bool> InsertProfit(DTOFabProfit profit);
        Task<bool> UpdateProfit(DTOFabProfit profit);
        Task<bool> DeleteProfit(int profitId);
        Task<IEnumerable<DTOFabProfit>> GetProfitsByDateRange(DateTime fromDate, DateTime toDate);
    }
}

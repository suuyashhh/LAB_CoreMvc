using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabPESummaryRepository
    {
        Task<DTOFabDatePE> GetProfitExpenseSummaryForDateRange(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<DTOFabMonthlyPE>> GetMonthlyProfitExpenseSummary();
    }
}

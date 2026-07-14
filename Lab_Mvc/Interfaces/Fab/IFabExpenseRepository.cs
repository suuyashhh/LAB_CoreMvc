using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabExpenseRepository
    {
        Task<bool> InsertExpense(DTOFabExpanse expense);
        Task<bool> UpdateExpense(DTOFabExpanse expense);
        Task<bool> DeleteExpense(int expenseId);
        Task<IEnumerable<DTOFabExpanse>> GetExpensesByDateRange(DateTime fromDate, DateTime toDate, bool onlyGeneral);
    }
}

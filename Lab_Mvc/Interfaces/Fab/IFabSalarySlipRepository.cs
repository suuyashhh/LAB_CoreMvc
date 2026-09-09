using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabSalarySlipRepository
    {
        Task<bool> SaveSalarySlip(DTOSalarySlip salarySlip);
        Task<IEnumerable<DTOSalarySlip>> GetSalarySlipsHistory();
        Task<IEnumerable<DTOSalarySlip>> GetHelperSalaryHistory(int userId);
        Task<IEnumerable<DTOSalarySlip>> GetSalarySlipsByDateRange(DateTime fromDate, DateTime toDate);
    }
}

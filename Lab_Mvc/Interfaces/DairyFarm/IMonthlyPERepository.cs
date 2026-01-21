// IMonthlyPERepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IMonthlyPERepository
    {
        Task<IEnumerable<MonthlyPEDto>> GetMonthlySummary(int userId);
    }
}
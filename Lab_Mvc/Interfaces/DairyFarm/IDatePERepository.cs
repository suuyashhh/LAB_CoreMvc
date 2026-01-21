// IDatePERepository.cs
using System.Threading.Tasks;
using Models.DairyFarm;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IDatePERepository
    {
        Task<DatePEDto> GetDateRangeSummary(DatePEQueryDto query);
    }
}
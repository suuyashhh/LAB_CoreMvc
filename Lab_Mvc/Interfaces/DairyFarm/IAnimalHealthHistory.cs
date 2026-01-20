using Models.DairyFarm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IAnimalHealthHistory
    {
        Task<IEnumerable<AnimalHealthSummaryDTO>> GetAllAnimalsWithHealthSummary(int userId);
        Task<IEnumerable<AnimalHealthRecordDTO>> GetAnimalHealthHistory(int userId, int animalId);
    }
}
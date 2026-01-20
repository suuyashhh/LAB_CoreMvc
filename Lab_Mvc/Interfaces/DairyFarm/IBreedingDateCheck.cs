using Models.DairyFarm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IBreedingDateCheck
    {
        Task<IEnumerable<BreedingAnimalSummaryDTO>> GetAllAnimalsWithBreedingSummary(int userId);
        Task<IEnumerable<BreedingRecordDTO>> GetAnimalBreedingHistory(int userId, int animalId);
    }
}
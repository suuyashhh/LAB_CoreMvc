using Models.DairyFarm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.DairyFarm
{
    public interface IDairyMasters
    {
        // Animals
        Task<IEnumerable<AnimalDto>> GetAnimalsByUser(int userId);
        Task<int> CreateAnimal(AnimalDto animal);
        Task<bool> UpdateAnimal(AnimalDto animal);
        Task<bool> DeleteAnimal(int animalId);

        // Feeds
        Task<IEnumerable<FeedDto>> GetFeedsByUser(int userId);
        Task<int> CreateFeed(FeedDto feed);
        Task<bool> UpdateFeed(FeedDto feed);
        Task<bool> DeleteFeed(int feedId);
    }
}

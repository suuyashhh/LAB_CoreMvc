using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Market;

namespace Lab_Mvc.Interfaces.Market
{
    public interface IVegetableRepository
    {
        Task<IEnumerable<Vegetable>> GetAllAsync();
        Task<Vegetable?> GetByIdAsync(int id);
        Task<int> AddAsync(Vegetable vegetable);
        Task<bool> UpdateAsync(Vegetable vegetable);
        Task<bool> DeleteAsync(int id);
    }
}

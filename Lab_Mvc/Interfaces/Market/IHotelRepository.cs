using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Market;

namespace Lab_Mvc.Interfaces.Market
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> GetAllAsync();
        Task<Hotel?> GetByIdAsync(int id);
        Task<int> AddAsync(Hotel hotel);
        Task<bool> UpdateAsync(Hotel hotel);
        Task<bool> DeleteAsync(int id);
    }
}

using Models.Shop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Shop
{
    public interface IShopUser
    {
        Task<IEnumerable<DTOShopLogin>> GetAll();
        Task<DTOShopLogin?> GetById(long userId);
        Task<long> Insert(DTOShopLogin model);
        Task<int> Update(DTOShopLogin model);
        Task<int> Delete(long userId);
    }
}

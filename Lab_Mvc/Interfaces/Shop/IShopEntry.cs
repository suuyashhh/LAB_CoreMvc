using Models.Shop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Shop
{
    public interface IShopEntry
    {
        Task<IEnumerable<DTOShopEntry>> GetAll(long userId, bool isPaid);
        Task<IEnumerable<DTOShopEntry>> GetAllTypesEntrys(long userId);
        Task<DTOShopEntry> GetById(long shopEntryId, long userId);
        Task<long> Insert(DTOShopEntry model);
        Task<int> Update(DTOShopEntry model);
        Task<int> Delete(long shopEntryId, long userId);
    }
}

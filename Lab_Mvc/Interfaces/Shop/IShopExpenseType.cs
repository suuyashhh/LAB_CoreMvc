using Models.Shop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Shop
{
    public interface IShopExpenseType
    {
        Task<IEnumerable<DTOShopExpenseType>> GetAll();
        Task<DTOShopExpenseType?> GetById(int exId);
        Task<int> Insert(DTOShopExpenseType model);
        Task<int> Update(DTOShopExpenseType model);
        Task<int> Delete(int exId);
    }
}

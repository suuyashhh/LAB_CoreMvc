using Models.Tejas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Tejas
{
    public interface ITejasShop
    {
        Task<IEnumerable<DTOTejasShop>> GetAll();
        Task<DTOTejasShop?> GetById(long shopId);
        Task<long> Insert(DTOTejasShop model);
        Task<int> Update(DTOTejasShop model);
        Task<int> Delete(long shopId);
    }
}

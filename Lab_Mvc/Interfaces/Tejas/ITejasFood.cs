using Models.Tejas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Tejas
{
    public interface ITejasFood
    {
        Task<IEnumerable<TejasFoodItem>> GetAll();
        Task<TejasFoodItem?> GetById(string id);
        Task<int> Insert(TejasFoodItem item);
        Task<int> Update(TejasFoodItem item);
        Task<int> Delete(string id);
    }
}

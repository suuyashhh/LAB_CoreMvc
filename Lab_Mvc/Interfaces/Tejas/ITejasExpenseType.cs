using Models.Tejas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Tejas
{
    public interface ITejasExpenseType
    {
        Task<IEnumerable<DTOTejasExpenseType>> GetAll();
        Task<DTOTejasExpenseType?> GetById(int exId);
        Task<int> Insert(DTOTejasExpenseType model);
        Task<int> Update(DTOTejasExpenseType model);
        Task<int> Delete(int exId);
    }
}

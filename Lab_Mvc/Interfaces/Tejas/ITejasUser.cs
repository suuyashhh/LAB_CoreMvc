using Models.Tejas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Tejas
{
    public interface ITejasUser
    {
        Task<IEnumerable<DTOTejasLogin>> GetAll();
        Task<DTOTejasLogin?> GetById(long userId);
        Task<long> Insert(DTOTejasLogin model);
        Task<int> Update(DTOTejasLogin model);
        Task<int> Delete(long userId);
    }
}

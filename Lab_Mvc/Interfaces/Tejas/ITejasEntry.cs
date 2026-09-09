using Models.Tejas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Tejas
{
    public interface ITejasEntry
    {
        Task<IEnumerable<DTOTejasEntry>> GetAll(long userId, bool isPaid, long? shopId = null);
        Task<IEnumerable<DTOTejasEntry>> GetAllTypesEntrys(long userId, System.DateTime? fromDate = null, System.DateTime? toDate = null, long? shopId = null);
        Task<DTOTejasEntry> GetById(long tejasEntryId, long userId);
        Task<long> Insert(DTOTejasEntry model);
        Task<int> Update(DTOTejasEntry model);
        Task<int> Delete(long tejasEntryId, long userId);
    }
}

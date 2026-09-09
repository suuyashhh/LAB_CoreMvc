using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabUsersRepository
    {
        Task<IEnumerable<DTOFabUsers>> GetAllHelpers();
        Task<bool> UpdateHelper(DTOFabUsers helper);
        Task<bool> DeleteHelper(int userId);
    }
}

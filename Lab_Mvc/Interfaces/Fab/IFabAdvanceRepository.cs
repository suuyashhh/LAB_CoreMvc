using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabAdvanceRepository
    {
        Task<IEnumerable<DTOFabExpanse>> GetAdvances();
        Task<IEnumerable<DTOFabExpanse>> GetHelperAdvanceHistory(int userId);
    }
}

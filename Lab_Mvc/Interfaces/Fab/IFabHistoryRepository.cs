using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabHistoryRepository
    {
        Task<IEnumerable<DTOFabHistory>> GetAllHistory();
    }
}

using System.Threading.Tasks;
using Models.Fab;

namespace Lab_Mvc.Interfaces.Fab
{
    public interface IFabLoginRepository
    {
        Task<DTOFabLogin?> LoginAdmin(string contact, string password);
        Task<DTOFabLogin?> LoginHelper(string contact, string password);
        Task<bool> RegisterHelper(DTOFabUsers helper);
    }
}

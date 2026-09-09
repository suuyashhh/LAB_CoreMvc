using Models.Tejas;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Tejas
{
    public interface ITejasLogin
    {
        Task<DTOTejasLogin?> Login(DTOTejasLogin login);
    }
}

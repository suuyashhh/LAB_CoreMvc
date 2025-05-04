using Models;

namespace Lab_Mvc.Interfaces
{
    public interface ILogin
    {
        Task<DTOLogin> Login(DTOLogin login);
    }
}

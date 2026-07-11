using Models.Lab;
using Models;

namespace Lab_Mvc.Interfaces.Lab
{
    public interface ILogin
    {
        Task<DTOLogin> Login(DTOLogin login);
    }
}


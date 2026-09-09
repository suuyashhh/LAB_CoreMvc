using Models.Lab;
using Models;

namespace Lab_Mvc.Interfaces.Lab
{
    public interface IAdmin
    {
        Task<IEnumerable<DTOAdmin>> GetCompanies();
    }
}


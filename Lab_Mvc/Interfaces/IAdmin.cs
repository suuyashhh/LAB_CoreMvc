using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IAdmin
    {
        Task<IEnumerable<DTOAdmin>> GetCompanies();
    }
}

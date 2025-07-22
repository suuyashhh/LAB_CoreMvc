using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IHome
    {
        Task<DTOHome> GetHomeById(string from_date, string to_date);
    }
}

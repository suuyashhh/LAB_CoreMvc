using Models.Lab;
using Models;

namespace Lab_Mvc.Interfaces.Lab
{
    public interface IHome
    {
        Task<DTOHome> GetHomeById(string from_date, string to_date, int comId);
    }
}


using Models.Lab;
using Models;

namespace Lab_Mvc.Interfaces.Lab
{
    public interface IFinance
    {
        Task<DTOFinance> GetFinanceById(string from_date, string to_date, int comId);
    }
}


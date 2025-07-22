using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IFinance
    {
        Task<DTOFinance> GetFinanceById(string from_date, string to_date);
    }
}

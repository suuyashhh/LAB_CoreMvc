using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IOtherExpense
    {
        Task<IEnumerable<DTOOtherExpense>> GetOtherExpense();
        Task<DTOOtherExpense> GetOtherExpenseById(long otherEx_id);
        Task SaveOtherExpense(DTOOtherExpense objOtherEx);
        Task EditOtherExpense(DTOOtherExpense objOtherEx, long otherEx_id);
        Task DeleteOtherExpense(long otherEx_id);
    }
}

using Models;

namespace Lab_Mvc.Interfaces
{
    public interface IOtherExpense
    {
        Task<IEnumerable<DTOOtherExpense>> GetOtherExpense(int comId);
        Task<DTOOtherExpense> GetOtherExpenseById(long otherEx_id);
        Task<List<DTOOtherExpense>> GetDateWiseOthMaterials(string from_date, string to_date);
        Task SaveOtherExpense(DTOOtherExpense objOtherEx);
        Task EditOtherExpense(DTOOtherExpense objOtherEx, long otherEx_id);
        Task DeleteOtherExpense(long otherEx_id);
    }
}

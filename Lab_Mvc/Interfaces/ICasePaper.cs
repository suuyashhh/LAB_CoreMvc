using Models;

namespace Lab_Mvc.Interfaces
{
    public interface ICasePaper
    {
        Task<IEnumerable<DTOCasePaper>> GetCasePapers(int comId);
        Task<DTOCasePaper> GetCasePaperById(long trn_no);
        Task SaveCasePaper(DTOCasePaper casepaper);
        Task EditCasePaper(DTOCasePaper casepaper, long trn_no);
        Task DeleteCasePaper(long trn_no);
        //Task<long> GenerateNewPatientId(Int64 comId);
    }
}

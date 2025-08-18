using Models;

namespace Lab_Mvc.Interfaces
{
    public interface ICasePaper
    {
        Task<IEnumerable<DTOCasePaper>> GetCasePapers(int comId);
        Task<DTOCasePaper> GetCasePaperById(long trn_no, int comId);
        Task<List<DTOCasePaper>> GetDateWiseCasePaper(string from_date, string to_date, int comId);
        Task SaveCasePaper(DTOCasePaper casepaper);
        Task EditCasePaper(DTOCasePaper casepaper, Int64 trn_no);
        Task DeleteCasePaper(long trn_no, int comId);
        //Task<long> GenerateNewPatientId(Int64 comId);
    }
}

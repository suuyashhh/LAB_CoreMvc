using Models.Farm;

namespace Lab_Mvc.Interfaces.Farm
{
    public interface IFarmEntry
    {
        Task<IEnumerable<DTOFarmEntry>> GetAll(long farmId, long userId, string entryTypeName);
        Task<IEnumerable<DTOFarmEntry>> GetAllTypesEntrys(long farmId, long userId);
        Task<DTOReportCalculation> GetReportCalculation(long farmId, long userId);
        Task<DTOFarmEntry> GetById(long farmEntryId, long farmId, long userId);
        Task<long> Insert(DTOFarmEntry model);
        Task<int> Update(DTOFarmEntry model);
        Task<int> Delete(long farmEntryId, long farmId, long userId);
    }
}
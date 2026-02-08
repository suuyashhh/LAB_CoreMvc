using Models.Farm;

namespace Lab_Mvc.Interfaces.Farm
{
    public interface IHomeFarm
    {
        Task<IEnumerable<DTOHomeFarm>> GetAll(string userId);
        Task<int> Insert(DTOHomeFarm model);
        Task<int> Update(DTOHomeFarm model);
        Task<int> Delete(int farmId, string userId);
    }
}

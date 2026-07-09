using Models.Market;

namespace Lab_Mvc.Interfaces.Market
{
    public interface IMarketVegitables
    {
        Task<List<DTOMarketVegitables>> GetVegetables();
        Task<string> SaveVegitable(string name);
    }
}

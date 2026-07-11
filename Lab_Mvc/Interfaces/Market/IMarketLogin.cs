using Models.Market;

namespace Lab_Mvc.Interfaces.Market
{
    public interface IMarketLogin
    {
        Task<DTOMarketLogin> LoginCheck(string contact, string pass);
    }
}

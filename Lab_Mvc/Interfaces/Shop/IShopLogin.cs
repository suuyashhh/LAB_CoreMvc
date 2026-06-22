using Models.Shop;
using System.Threading.Tasks;

namespace Lab_Mvc.Interfaces.Shop
{
    public interface IShopLogin
    {
        Task<DTOShopLogin?> Login(DTOShopLogin login);
    }
}

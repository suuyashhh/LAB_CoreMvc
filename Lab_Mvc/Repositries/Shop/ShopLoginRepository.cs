using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Shop;
using SmartParking.Interfaces;
using SmartParking.Repositories;

namespace Lab_Mvc.Repositries.Shop
{
    public class ShopLoginRepository : DapperRepositoryBase, IShopLogin
    {
        public ShopLoginRepository(DapperContext context) : base(context)
        {
        }
    }
}

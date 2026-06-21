using Lab_Mvc.Interfaces.Shop;
using Microsoft.AspNetCore.Mvc;
using SmartParking.Interfaces;
using System.Numerics;

namespace Lab_Mvc.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopLoginController : Controller
    {
        private readonly IShopLogin _iShopLogin;
        private readonly IConfiguration _config;
        public IActionResult Index()
        {
            return View();
        }
    }
}

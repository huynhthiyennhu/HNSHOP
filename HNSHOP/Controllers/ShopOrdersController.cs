using Microsoft.AspNetCore.Mvc;

namespace HNSHOP.Controllers
{
    public class ShopOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

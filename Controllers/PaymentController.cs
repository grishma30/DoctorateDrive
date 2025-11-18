using Microsoft.AspNetCore.Mvc;

namespace DoctorateDrive.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

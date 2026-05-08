using Microsoft.AspNetCore.Mvc;

namespace CMMS.Controllers
{
    public class MenuInventarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

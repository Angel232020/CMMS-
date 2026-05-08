using Microsoft.AspNetCore.Mvc;

namespace CMMS.Controllers
{
    public class MenuTecnico : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace CMMS.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
            //return Content("Llegué al menú");
        }


    }
}

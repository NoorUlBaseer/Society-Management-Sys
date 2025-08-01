using Microsoft.AspNetCore.Mvc;

namespace SocietyMng.Areas.Admin.Controllers
{
    public class AssetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Owner,Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

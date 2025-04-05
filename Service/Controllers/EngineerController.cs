using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Authorize(Roles = "Engineer")]
    public class EngineerController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Панель инженера";
            return View();
        }
    }
}

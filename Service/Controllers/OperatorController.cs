using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Authorize(Roles = "Operator")]
    public class OperatorController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Панель оператора";
            return View();
        }
    }
   
}

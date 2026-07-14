using Microsoft.AspNetCore.Mvc;

namespace RecruitFlow.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Login", "Account");

        public IActionResult Error() => View();
    }
}

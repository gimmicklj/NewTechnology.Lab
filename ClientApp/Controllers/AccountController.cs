using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult UserLogin()
        {
            return View();
        }
    }
}

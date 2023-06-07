using Microsoft.AspNetCore.Mvc;

namespace ConsumerWebAPI.Controllers
{
    public class AjaxController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

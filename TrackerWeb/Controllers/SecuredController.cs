using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrackerWeb.Controllers
{
    public class SecuredController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}

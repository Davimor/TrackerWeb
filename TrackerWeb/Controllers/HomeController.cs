using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Principal;
using TrackerWeb.Models;
using Microsoft.AspNetCore.Http;

namespace TrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        private readonly bool _login;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (HttpContext.Request.Cookies.ContainsKey("loginTracker"))
            {
                return View();
            }
            else
            {
                return View("Login");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (HttpContext.Request.Cookies.ContainsKey("loginTracker"))
            {
                return View("Index");
            }
            else
            {
                return View();
            }
        }



        //POST: Logout
        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Response.Cookies.Delete("loginTracker");
                return RedirectToAction("Login");
            }
            catch
            {
                throw;
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM entity)
        {
            try
            {
                LoginVM? user = null;
                using (DapperAccess db = new DapperAccess(_configuration))
                {
                    user = db.GetSimpleData<LoginVM>("SELECT * FROM USUARIOS WHERE IdUser = @Nombre", entity).FirstOrDefault();
                }

                if (user != null)
                {
                    if (user.pass == entity.pass)
                    {
                        //login OK crear cookie y todo eso
                        CookieOptions cookieOptions = new CookieOptions();
                        cookieOptions.Secure = true;
                        if (entity.isRemember)
                        {
                            cookieOptions.Expires = DateTime.Today.AddDays(15).AddHours(23).AddMinutes(59);
                        }
                        else
                        {
                            cookieOptions.Expires = DateTime.Today.AddHours(23).AddMinutes(59);
                        }
                        TempData["ErrorMSG"] = null;
                        HttpContext.Response.Cookies.Append("loginTracker", user.Nombre + " " + user.Apellidos, cookieOptions);
                        if (user.Administrador)
                        {
                            HttpContext.Response.Cookies.Append("trackerAdmin", user.Administrador.ToString(), cookieOptions);
                        }
                        if (user.AsignaCasos)
                        {
                            HttpContext.Response.Cookies.Append("trackerAsign", user.AsignaCasos.ToString(), cookieOptions);
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMSG"] = "Acceso denegado! Revise sus credenciales";
                        return View("Login", entity);
                    }
                }
                else
                {
                    //Login Fail
                    TempData["ErrorMSG"] = "Acceso denegado! Revise sus credenciales";
                    return View(entity);
                }
            }
            catch
            {
                throw;

            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
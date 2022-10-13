using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TrackerWeb.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Identity;
using DTO;

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
            //if (HttpContext.Request.Cookies.ContainsKey("loginTracker"))
            if (User.Identity.IsAuthenticated)
            {
                
                return View();
            }
            else
            {
                string a = User.Identity.Name;
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
                return View();
        }



        //POST: Logout
        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
                        TempData["ErrorMSG"] = null;

                        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Nombre + " " + user.Apellidos) };
                        claims.Add(new Claim(ClaimTypes.Sid, user.IdUser));

                        if (user.Administrador)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
                        }
                        if (user.AsignaCasos)
                        {
                            claims.Add(new Claim(ClaimTypes.Actor, "Casos"));
                        }
                        var authProperties = new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        };

                        if (entity.isRemember)
                            authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(23);

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

                        _logger.LogInformation($"User {user.IdUser} logged in at {DateTime.Now}");
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
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
using System.Text.Json;
using Microsoft.CodeAnalysis;
using static System.Net.WebRequestMethods;

namespace TrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        private HomeViewModel _model;

        private Empleado user;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into HomeController");
            _configuration = configuration;

            ClaimsIdentity identity = (ClaimsIdentity)contextAccessor.HttpContext.User.Identity;
            var userdata = identity.FindFirst(ClaimTypes.UserData);
            if (userdata != null)
            {
                user = JsonSerializer.Deserialize<Empleado>(userdata.Value);
            }
        }

        public IActionResult Index(string ReturnUrl = "")
        {
            //if (HttpContext.Request.Cookies.ContainsKey("loginTracker"))
            if (User.Identity.IsAuthenticated)
            {
                _model = new HomeViewModel(_configuration);
                return View(_model);
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

        //POST: Logout
        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _logger.LogInformation($"User {user.IdUser} singout");
                return RedirectToAction("");
            }
            catch (Exception ex)
            {
                var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                var method = this.ControllerContext.RouteData.Values["action"].ToString();
                _logger.LogError($"{controller}/{method}->  {user.IdUser} : {ex.Message}");
                throw;
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Empleado entity)
        {
            try
            {
                Empleado? user = null;
                using (DapperAccess db = new DapperAccess(_configuration))
                {
                    user = db.GetSimpleData<Empleado>("SELECT * FROM Empleados WHERE IdUser = @Nombre", entity).FirstOrDefault();
                }

                if (user != null)
                {
                    if (user.pass == entity.pass)
                    {
                        //login OK crear cookie y todo eso
                        CookieOptions cookieOptions = new CookieOptions();
                        cookieOptions.Secure = true;
                        TempData["ErrorMSG"] = null;

                        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName) };
                        claims.Add(new Claim(ClaimTypes.Sid, user.IdUser));

                        claims.Add(new Claim(ClaimTypes.UserData, JsonSerializer.Serialize(user)));

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
            catch (Exception ex)
            {
                var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                var method = this.ControllerContext.RouteData.Values["action"].ToString();
                _logger.LogError($"{controller}/{method}->  {entity.IdUser} : {ex.Message}");
                throw;

            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
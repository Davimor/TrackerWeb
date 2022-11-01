using Microsoft.AspNetCore.Mvc;
using TrackerWeb.Models;
using System.Security.Claims;
using DTO;
using System.Text.Json;

namespace TrackerWeb.Controllers
{
    public class ParteController : Controller
    {
        private readonly ILogger<ParteController> _logger;

        private readonly IConfiguration _configuration;

        private ParteViewModel _model;

        private Empleado _user;

        public ParteController(ILogger<ParteController> logger, IConfiguration configuration, IHttpContextAccessor contextAccessor) {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into ParteController");
            _configuration = configuration;

            ClaimsIdentity identity = (ClaimsIdentity)contextAccessor.HttpContext.User.Identity;
            var userdata = identity.FindFirst(ClaimTypes.UserData);
            if (userdata != null)
            {
                _user = JsonSerializer.Deserialize<Empleado>(userdata.Value);
            }

            _model = new ParteViewModel(_configuration,_user);
        }


        public IActionResult Index()
        {
            return View(_model);
        }
    }
}

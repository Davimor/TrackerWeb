using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using TrackerWeb.Models;

namespace TrackerWeb.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfiguration Configuration { get; set; }
        public ClienteViewModel model { get; set; }
        private Empleado user;

        public ClienteController(ILogger<HomeController> logger, IConfiguration _configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into ClienteController");
            Configuration = _configuration;
            model = new ClienteViewModel(Configuration);
            ClaimsIdentity identity = (ClaimsIdentity)contextAccessor.HttpContext.User.Identity;
            var userdata = identity.FindFirst(ClaimTypes.UserData);
            if (userdata != null)
            {
                user = JsonSerializer.Deserialize<Empleado>(userdata.Value);
            }
        }

        public IActionResult Index()
        {
            return View(model);
        }

        [HttpPost]
        public JsonResult Create(Cliente cliente)
        {
           
            cliente.usuario_modificacion = user.IdUser;

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                db.Execute(@"INSERT INTO [dbo].[CLIENTES]
           ([NOMBRE]
           ,[APELLIDOS]
           ,[DIRECCION]
           ,[POBLACION]
           ,[CODPOSTAL]
           ,[PROVINCIA]
           ,[TELFIJO]
           ,[TELMOVIL]
           ,[EMAIL]
           ,[CARGO]
           ,[usuario_modificacion]
           ,[fecha_modificacion]
           ,[Contactos]
           ,[CODFACTUSOL]
           ,[TIPO])
     VALUES
           (@NOMBRE
           ,@APELLIDOS
           ,@DIRECCION
           ,@POBLACION
           ,@CODPOSTAL
           ,@PROVINCIA
           ,@TELFIJO
           ,@TELMOVIL
           ,@EMAIL
           ,@CARGO
           ,@usuario_modificacion
           ,GETDATE()
           ,@Contactos
           ,@CODFACTUSOL
           ,@TIPO)", cliente);
            }
            return Json(new ClienteViewModel(Configuration));
        }

        [HttpPost]
        public JsonResult Edit(Cliente cliente)
        {
            cliente.usuario_modificacion = user.IdUser;
            cliente.fecha_modificacion = DateTime.Now;

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                var antiguo = model.Clientes.Where(x => x.IDCLIENTE == cliente.IDCLIENTE).First();

                var cambios = Helper.GetChanges(antiguo, cliente, new string[] { "usuario_modificacion", "fecha_modificacion", "DESTIPO", "DESCARGO" });

                db.Execute(@"UPDATE [dbo].[CLIENTES]
   SET [NOMBRE] = @NOMBRE
      ,[APELLIDOS] = @APELLIDOS
      ,[DIRECCION] = @DIRECCION
      ,[POBLACION] = @POBLACION
      ,[CODPOSTAL] = @CODPOSTAL
      ,[PROVINCIA] = @PROVINCIA
      ,[TELFIJO] = @TELFIJO
      ,[TELMOVIL] = @TELMOVIL
      ,[EMAIL] = @EMAIL
      ,[CARGO] = @CARGO
      ,[usuario_modificacion] = @usuario_modificacion
      ,[fecha_modificacion] = GETDATE()
      ,[Contactos] = @Contactos
      ,[CODFACTUSOL] = @CODFACTUSOL
      ,[TIPO] = @TIPO
 WHERE IDCLiente = @IDCLIENTE", cliente);

            }
            return Json(new ClienteViewModel(Configuration));
        }
    }
}

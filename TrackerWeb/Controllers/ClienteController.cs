using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using TrackerWeb.Models;

namespace TrackerWeb.Controllers
{
    public class ClienteController : Controller
    {
        public IConfiguration Configuration { get; set; }
        public ClienteViewModel model { get; set; }

        public ClienteController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            model = new ClienteViewModel(Configuration);
        }

        public IActionResult Index()
        {
            return View(model);
        }

        [HttpPost]
        public JsonResult Create(Cliente cliente)
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.Sid);
            cliente.usuario_modificacion = claim.Value;

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
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.Sid);
            cliente.usuario_modificacion = claim.Value;
            cliente.fecha_modificacion = DateTime.Now;

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                var antiguo = model.Clientes.Where(x => x.IDCLIENTE == cliente.IDCLIENTE).First();

                var cambios = Helper.GetChanges(antiguo, cliente);

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

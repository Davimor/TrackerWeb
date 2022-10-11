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
    public class AvisoController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public AvisoViewModel model { get; set; }

        public AvisoController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            model = new AvisoViewModel(Configuration);
        }

        // GET: AvisoController
        public ActionResult Index()
        {
            return View("Index", model);
        }

        // GET: AvisoController/Details/5
        [HttpPost]
        public JsonResult GetHistorial(int id)
        {
            model.GetHistorial(id);
            return Json(model.HistorialAviso);
        }

        // GET: AvisoController/Create
        [HttpPost]
        public JsonResult Create(Aviso aviso)
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.Sid);
            aviso.usuario_modificacion = claim.Value;

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                aviso.NUMCASO = db.GetSimpleData<string>("SELECT [dbo].[CrearTicketCaso]()").First();
                db.Execute(@"INSERT INTO [dbo].[CASOS]
           ([NUMCASO]
           ,[CLIENTE]
           ,[ESTADO]
           ,[TIPO]
           ,[ORIGEN]
           ,[FUENTE]
           ,[Prioridad]
           ,[FECHA]
           ,[DESCRIPCION]
           ,[usuario_consulta]
           ,[usuario_modificacion]
           ,[fecha_modificacion])
     VALUES
           (@NUMCASO
           ,@IDCLIENTE
           ,@ESTADO
           ,@TIPO
           ,@ORIGEN
           ,@FUENTE
           ,@Prioridad
           ,@FECHA
           ,@DESCRIPCION
           ,''
           ,@usuario_modificacion
           ,GETDATE())", aviso);
                aviso.IDCASO = db.GetSimpleData<int>("SELECT IDCASO FROM CASOS WHERE NUMCASO = @NUMCASO", aviso).First();
                HistorialAviso h = new HistorialAviso()
                {
                    CASO = aviso.IDCASO.Value,
                    FECHA = DateTime.Now,
                    COMENTARIO = "Alta Aviso",
                    USUARIO = aviso.usuario_modificacion
                };

                db.Execute(@"INSERT INTO [dbo].[HISTORICOCASOS]
           ([CASO]
           ,[FECHA]
           ,[COMENTARIO]
           ,[USUARIO])
     VALUES
           (@CASO
           ,@FECHA
           ,@COMENTARIO
           ,@USUARIO)", h);
            }

            return Json(new AvisoViewModel(Configuration));
        }

        // GET: AvisoController/Create
        [HttpPost]
        public JsonResult Edit(Aviso aviso)
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.Sid);
            aviso.usuario_modificacion = claim.Value;

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                var antiguo=model.Avisos.Where(x => x.IDCASO == aviso.IDCASO).First();

                var cambios = Helper.GetChanges(aviso,antiguo);
                db.Execute(@"UPDATE [dbo].[CASOS]
   SET [CLIENTE] = @CLIENTE
      ,[ESTADO] = @ESTADO
      ,[TIPO] = @TIPO
      ,[ORIGEN] = @ORIGEN
      ,[FUENTE] = @FUENTE
      ,[Prioridad] = @Prioridad
      ,[FECHA] = @FECHA
      ,[DESCRIPCION] = @DESCRIPCION
      ,[usuario_modificacion] = @usuario_modificacion
      ,[fecha_modificacion] = GETDATE()
WHERE IDCASO = @IDCASO", aviso);
                HistorialAviso h = new HistorialAviso()
                {
                    CASO = aviso.IDCASO.Value,
                    FECHA = DateTime.Now,
                    COMENTARIO = cambios,
                    USUARIO = aviso.usuario_modificacion
                };
                db.Execute(@"INSERT INTO [dbo].[HISTORICOCASOS]
           ([CASO]
           ,[FECHA]
           ,[COMENTARIO]
           ,[USUARIO])
     VALUES
           (@CASO
           ,@FECHA
           ,@COMENTARIO
           ,@USUARIO)", h);
            }

            return Json(new AvisoViewModel(Configuration));
        }

    }
}

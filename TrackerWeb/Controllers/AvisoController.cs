using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using TrackerWeb.Models;

namespace TrackerWeb.Controllers
{
    [Authorize]
    public class AvisoController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public AvisoViewModel model { get; set; }

        public bool AsignaCasos { get; set; } = false;

        public string user { get; set; } = "";

        public AvisoController(IConfiguration _configuration, IHttpContextAccessor contextAccessor)
        {
            Configuration = _configuration;
            ClaimsIdentity identity = (ClaimsIdentity)contextAccessor.HttpContext.User.Identity;
            var claim = identity.FindFirst(ClaimTypes.Actor);
            AsignaCasos = claim != null;

            claim = identity.FindFirst(ClaimTypes.Sid);
            user = claim.Value;

            model = new AvisoViewModel(Configuration, AsignaCasos);
        }

        // GET: AvisoController
        public ActionResult Index()
        {
            return View(model);
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
            aviso.usuario_modificacion = user;

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

            return Json(new AvisoViewModel(Configuration, AsignaCasos));
        }

        // GET: AvisoController/Create
        [HttpPost]
        public JsonResult Edit(Aviso aviso)
        {
            aviso.usuario_modificacion = user;

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                var antiguo = model.Avisos.Where(x => x.IDCASO == aviso.IDCASO).First();

                var cambios = Helper.GetChanges<Aviso>(antiguo, aviso, new string[] { "FECHA", "DESESTADO", "DESTIPO", "DESORIGEN", "DESFUENTE", "fecha_modificacion", "CLIENTE", "NUMCASO", "usuario_modificacion" });
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

            return Json(new AvisoViewModel(Configuration, AsignaCasos));
        }

        [HttpPost]
        public JsonResult Asignar(string employeeID, IEnumerable<Aviso> avisos)
        {

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                foreach (var a in avisos)
                {
                    var exists = !string.IsNullOrEmpty(db.GetSimpleData<string>("SELECT IDCASO FROM AsignacionCasos WHERE IdCaso= @IdCaso", new { IdCaso = a.IDCASO }).FirstOrDefault());
                    var accion = "asignado";
                    if (!exists)
                    {
                        db.Execute("INSERT INTO AsignacionCasos VALUES (@IdCaso,@EmployeeID)", new { IdCaso = a.IDCASO, EmployeeID = employeeID });
                    }
                    else
                    {
                        accion = "reasignado";
                        db.Execute("UPDATE AsignacionCasos SET EmployeeID = @EmployeeID WHERE IDCASO = @IDCASO", new { IdCaso = a.IDCASO, EmployeeID = employeeID });
                    }

                    var emp = model.Empleados.Where(x => x.EmployeeID == employeeID).First();

                    HistorialAviso h = new HistorialAviso()
                    {
                        CASO = a.IDCASO.Value,
                        FECHA = DateTime.Now,
                        COMENTARIO = $"Aviso {accion} a {emp.FirstName} {emp.LastName}",
                        USUARIO = user
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
            }

            return Json(new AvisoViewModel(Configuration, AsignaCasos));
        }

    }
}

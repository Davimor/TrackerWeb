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
        public JsonResult GetHistoral(int id)
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
                aviso.IDCASO = db.GetSimpleData<int>("SELECT IDCASO FROM CASOS WHERE NUMCASO = @NUMCASO",aviso).First();
                HistorialAviso h = new HistorialAviso() {
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
           ,@USUARIO)",h);
            }

            return Json(new AvisoViewModel(Configuration));
        }


        // GET: AvisoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AvisoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AvisoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AvisoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

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
        public ActionResult Details(int id)
        {
            return View();
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
           ([dbo].[CrearTicketCaso]()
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

using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using TrackerWeb.Models;

namespace TrackerWeb.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfiguration Configuration { get; set; }

        private Empleado user;

        public DocumentController(ILogger<HomeController> logger, IConfiguration _configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into AvisoController");

            Configuration = _configuration;

            ClaimsIdentity identity = (ClaimsIdentity)contextAccessor.HttpContext.User.Identity;
            var userdata = identity.FindFirst(ClaimTypes.UserData);
            if (userdata != null)
            {
                user = JsonSerializer.Deserialize<Empleado>(userdata.Value);
            }
        }
        [HttpPost]
        public JsonResult UploadFiles(long idcaso)
        {
            var files = Request.Form.Files;
            List<Documento> docs = new List<Documento>();
            if (files != null)
            {
                foreach (var f in files)
                {
                    if (f.Length > 0)
                    {
                        docs.Add(new Documento(f, user));
                    }
                }
            }

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                foreach (var doc in docs)
                {
                    //Inserto DOC
                    var iddoc = db.ExecuteScalar(@"INSERT INTO [dbo].[DOCUMENTOS]
           ([Name]
           ,[ContentType]
           ,[Data]
           ,[UploadUser]
           ,[UploadDate])
     VALUES
           (@Name
           ,@ContentType
           ,@Data
           ,@UploadUser
           ,@UploadDate)", doc);
                    //Inserto relacion doc y caso
                    db.Execute("INSERT INTO CASODOCUMENTO (IDCASO,IDDOCUMENTO) VALUES ( @Idcaso,@IdDoc)", new { Idcaso = idcaso, IdDoc = iddoc });

                }
            }

            return Json(true);
        }

        [HttpGet]
        public ActionResult GetFile(int id) {
            var doc = new Documento();
            using (DapperAccess db = new DapperAccess(Configuration))
            {
                doc = db.GetSimpleData<Documento>("SELECT * FROM DOCUMENTOS WHERE id = @id", new { id = id }).First();
            }
            return File(doc.Data, doc.ContentType, doc.Name);
        }



    }
}

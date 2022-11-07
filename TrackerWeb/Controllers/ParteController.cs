using Microsoft.AspNetCore.Mvc;
using TrackerWeb.Models;
using System.Security.Claims;
using DTO;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;

namespace TrackerWeb.Controllers
{
    public class ParteController : Controller
    {
        private readonly ILogger<ParteController> _logger;

        private readonly IConfiguration _configuration;

        private ParteViewModel _model;

        private Empleado _user;

        public ParteController(ILogger<ParteController> logger, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into ParteController");
            _configuration = configuration;

            ClaimsIdentity identity = (ClaimsIdentity)contextAccessor.HttpContext.User.Identity;
            var userdata = identity.FindFirst(ClaimTypes.UserData);
            if (userdata != null)
            {
                _user = JsonSerializer.Deserialize<Empleado>(userdata.Value);
            }

            _model = new ParteViewModel(_configuration, _user);
        }


        public IActionResult Index()
        {
            return View(_model);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveParte(string parte)
        {
            var files = Request.Form.Files;
            List<Documento> docs = new List<Documento>();
            if (files != null)
            {
                foreach (var f in files)
                {
                    if (f.Length > 0)
                    {
                        docs.Add(new Documento(f, _user));
                    }
                }
            }
            var nuevo = JsonSerializer.Deserialize<Parte>(parte);
            nuevo.idUser = _user.IdUser;
            using (DapperAccess db = new DapperAccess(_configuration))
            {
                var idParte = db.ExecuteScalar(@"INSERT INTO [dbo].[INTERVENCIONES]
           ([NUMINTERVENCION]
           ,[IDCLIENTE]
           ,[FECHA]
           ,[TIPOINTERVENCION]
           ,[IdUser]
           ,[Observaciones])
     VALUES
           (@numParte
           ,@idCliente
           ,@fecha
           ,@tipoIntervencion
           ,@IdUser
           ,@observaciones)", nuevo);
                foreach (var t in nuevo.trabajos) {
                    t.idIntervencion = Decimal.ToInt32((decimal)idParte);
                    t.tecnico = _model.Tecnicos.Where(x => x.IdUser == t.tecnico).First().EmployeeID;
                }
                var trabajos = db.Execute(@"INSERT INTO [dbo].[HistoricoIntervenciones]
           ([IdIntervencion]
           ,[TipoTrabajo]
           ,[EstadoTrabajo]
           ,[Descripcion]
           ,[IdEmp])
     VALUES
           (@idIntervencion
           ,@tipoTrabajo
           ,@estadoTrabajo
           ,@descripcion
           ,@tecnico)", nuevo.trabajos);

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
                    db.Execute("INSERT INTO INTERVENCIONDOCUMENTO ([IdIntervencion], [IdDoc]) VALUES ( @IdIntervencion,@IdDoc)", new { IdIntervencion = idParte, IdDoc = iddoc });
                }


                return Json(true);
            }
        }

    }
}

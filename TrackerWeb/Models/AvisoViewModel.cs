using DTO;
using Newtonsoft.Json;
using System.Configuration;

namespace TrackerWeb.Models
{
    public class AvisoViewModel
    {
        private IConfiguration Configuration { get; set; }
        public List<DTO.Aviso> Avisos { get; set; }
        public List<DTO.Cliente> Clientes { get; set; }
        public List<DTO.KeyValue> Fuentes { get; set; }
        public List<DTO.KeyValue> Estados { get; set; }
        public List<DTO.KeyValue> Tipos { get; set; }
        public List<DTO.KeyValue> Origenes { get; set; }
        public List<DTO.HistorialAviso> HistorialAviso { get; set; }
        public bool AsignaCasos { get; set; } = false;
        public List<DTO.Aviso> Seleccionados { get; set; }
        public List<DTO.Empleado> Empleados { get; set; }
        public List<DTO.Documento> DocumentosAviso { get; set; }

        public AvisoViewModel(IConfiguration _Configuration, bool asignaCasos = false, bool ultimos = true)
        {
            Configuration = _Configuration;
            AsignaCasos = asignaCasos;
            var cond = "";
            if (ultimos)
            {
                cond = " WHERE FECHA >= DATEADD(year, -1, GETDATE()) ";
            }

            using (DapperAccess db = new DapperAccess(Configuration))
            {
                Avisos = db.GetSimpleData<DTO.Aviso>(@$"SELECT a.IDCASO,
a.FECHA,
a.CLIENTE 'IDCLIENTE',
cli.NOMBRE 'CLIENTE',
a.ESTADO 'ESTADO',
e.DESCRIPCION 'DESESTADO',
a.TIPO 'TIPO',
t.DESCRIPCION 'DESTIPO',
a.ORIGEN 'ORIGEN',
o.DESCRIPCION 'DESORIGEN',
STRING_ESCAPE(cast(a.DESCRIPCION as nvarchar(MAX)),'json') 'DESCRIPCION',
a.Prioridad,
a.NUMCASO,
a.FUENTE 'FUENTE',
f.DESCRIPCION 'DESFUENTE',
cast(ISNULL(ac.IdCaso, 0) as bit) 'ASIGNADO',
emp.EmployeeID,
emp.FirstName + ' ' + emp.LastName 'EMPLEADO',
a.fecha_modificacion
FROM CASOS a
LEFT JOIN CLIENTES cli ON a.CLIENTE = cli.IDCLIENTE 
JOIN ESTADOS e On e.IDESTADO = a.ESTADO
JOIN TIPOS t ON t.IDTIPO = a.TIPO
JOIN ORIGEN o ON o.IDORIGEN = a.ORIGEN
JOIN FUENTES f ON f.IdFuente = a.FUENTE
LEFT JOIN AsignacionCasos ac ON ac.IdCaso = a.IDCASO
LEFT JOIN Empleados emp ON emp.EmployeeID = ac.EmployeeID
{cond}
ORDER BY FECHA DESC").ToList();

                Clientes = db.GetSimpleData<DTO.Cliente>(@"SELECT  [IDCLIENTE]
      ,[NOMBRE]
      ,[APELLIDOS]
      ,[DIRECCION]
      ,[POBLACION]
      ,[CODPOSTAL]
      ,[PROVINCIA]
      ,[TELFIJO]
      ,[TELMOVIL]
      ,[EMAIL]
      ,[CARGO]
	  ,car.DESCRIPCION 'DESCARGO'
      ,[usuario_consulta]
      ,[fecha_consulta]
      ,[usuario_modificacion]
      ,[fecha_modificacion]
      ,STRING_ESCAPE(cast([CONTACTOS] as nvarchar(MAX)),'json') as [CONTACTOS]
      ,[CODFACTUSOL]
      ,[TIPO]
	  ,tc.DESCRIPCION 'DESTIPO'FROM CLIENTES c
	  LEFT JOIN TIPOCLIENTE tc on tc.IDTIPOCLI = c.TIPO
	  LEFT JOIN CARGOS car on car.IDCARGO = c.CARGO");

                Fuentes = db.GetSimpleData<KeyValue>("SELECT idFuente 'clave', DESCRIPCION 'valor' FROM FUENTES");
                Estados = db.GetSimpleData<KeyValue>("SELECT idEstado 'clave', DESCRIPCION 'valor' FROM ESTADOS");
                Tipos = db.GetSimpleData<KeyValue>("SELECT idTipo 'clave', DESCRIPCION 'valor' FROM TIPOS");
                Origenes = db.GetSimpleData<KeyValue>("SELECT idOrigen 'clave', DESCRIPCION 'valor' FROM ORIGEN");

                Empleados = db.GetSimpleData<Empleado>(@"SELECT e.EmployeeID,e.FirstName,e.LastName
FROM Empleados e
LEFT join AsignacionCasos ea ON ea.EmployeeID = e.EmployeeID 
GROUP BY e.EmployeeID,e.FirstName,e.LastName");

                foreach (Empleado emp in Empleados)
                {
                    emp.Abiertos = Avisos.Where(x => x.EmployeeID == emp.EmployeeID && !x.DESESTADO.StartsWith("Cerrada")).Count();
                    emp.Cerrados30D = Avisos.Where(x => x.EmployeeID == emp.EmployeeID && x.DESESTADO.StartsWith("Cerrada") && x.fecha_modificacion >= DateTime.Today.AddDays(-30)).Count();
                }
            }
        }

        internal void GetHistorial(int id)
        {
            using (DapperAccess db = new DapperAccess(Configuration))
            {
                HistorialAviso = db.GetSimpleData<HistorialAviso>("SELECT * FROM HISTORICOCASOS WHERE CASO = @id", new { id = id });
            }
        }
        internal void GetDocumentos(int id)
        {
            using (DapperAccess db = new DapperAccess(Configuration))
            {
                DocumentosAviso = db.GetSimpleData<Documento>(@"SELECT d.Id, d.Name, d.ContentType, d.UploadUser,d.UploadDate FROM Documentos d
  JOIN CASODOCUMENTO cd ON cd.idDocumento = d.id
  WHERE cd.Idcaso = @id", new { id = id });
            }
        }
    }
}

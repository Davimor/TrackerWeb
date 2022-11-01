using DTO;
using System.Configuration;

namespace TrackerWeb.Models
{
    public class HomeViewModel
    {
        public IConfiguration configuration { get; set; }
        public List<Aviso> Avisos { get; set; } = new List<Aviso>();

        public List<KeyValue> Estados { get; set; } = new List<KeyValue>();

        public Empleado user { get; set; }

        public HomeViewModel(IConfiguration _configuration, Empleado _user)
        {
            configuration = _configuration;

            using (DapperAccess db = new DapperAccess(_configuration))
            {
                Avisos = db.GetSimpleData<DTO.Aviso>(@"SELECT  a.IDCASO,
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
ORDER BY FECHA DESC").ToList();
                Estados = db.GetSimpleData<KeyValue>("SELECT idEstado 'clave', DESCRIPCION 'valor' FROM ESTADOS");
                user = _user;
            }

            this.user = user;
        }
    }
}
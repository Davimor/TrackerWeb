using DTO;
using System.Configuration;

namespace TrackerWeb.Models
{
    public class ParteViewModel
    {
        public IConfiguration configuration { get; set; }

        public Empleado user { get; set; }
        public List<DTO.Cliente> Clientes { get; set; }
        public List<DTO.Empleado> Tecnicos { get; set; }

        public ParteViewModel(IConfiguration _configuration, Empleado _user)
        {
            configuration = _configuration;
            user = _user;
            using (DapperAccess db = new DapperAccess(configuration))
            {

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

                Tecnicos = db.GetSimpleData<DTO.Empleado>(@"SELECT  * FROM Empleados");
            }

        }
    }
}

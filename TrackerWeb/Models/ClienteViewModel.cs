using DTO;
using System.Configuration;

namespace TrackerWeb.Models
{
    public class ClienteViewModel
    {
        IConfiguration configuration { get; set; }
        public List<DTO.Cliente> Clientes { get; set; }
        public List<DTO.KeyValue> Cargos { get; set; }
        public List<DTO.KeyValue> TipoClientes { get; set; }
        public List<DTO.KeyValue> Provincias { get; set; }

        public ClienteViewModel(IConfiguration _configuration)
        {
            this.configuration = _configuration;
            using (DapperAccess db = new DapperAccess(configuration))
            {
                Clientes = db.GetSimpleData<Cliente>(@"SELECT  [IDCLIENTE]
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
                Cargos = db.GetSimpleData<KeyValue>("SELECT IDCARGO 'clave', DESCRIPCION 'valor' FROM CARGOS");
                TipoClientes = db.GetSimpleData<KeyValue>("SELECT IDTIPOCLI 'clave', DESCRIPCION 'valor' FROM TIPOCLIENTE");
                Provincias = db.GetSimpleData<KeyValue>("SELECT IdProvincia 'clave', Provincia 'valor' FROM PROVINCIAS");
            }
        }

    }
}

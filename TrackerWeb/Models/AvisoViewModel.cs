using Newtonsoft.Json;
using System.Configuration;

namespace TrackerWeb.Models
{
    public class AvisoViewModel
    {
        public List<DTO.Aviso> Avisos { get; set; }
        public List<DTO.Cliente> Clientes { get; set; }

        public AvisoViewModel(IConfiguration _Configuration)
        {

            using (DapperAccess db = new DapperAccess(_Configuration))
            {
                Avisos = db.GetSimpleData<DTO.Aviso>("SELECT TOP 100 [IDCASO]" +
                    ",[NUMCASO]" +
                    ",[CLIENTE]" +
                    ",[CONTACTO]" +
                    ",[ESTADO]" +
                    ",[TIPO]" +
                    ",[ORIGEN]" +
                    ",[FUENTE]" +
                    ",[Prioridad]" +
                    ",[FECHA]" +
                    ",replace(replace(replace(cast([DESCRIPCION] as nvarchar),char(13),''),char(10),''),'\"','') as [DESCRIPCION]" +
                    ",[usuario_consulta]   " +
                    ",[fecha_consulta] "+
                    ",[usuario_modificacion]" +
                    ",[fecha_modificacion] " +
                    "FROM CASOS", null).ToList();

                Clientes = db.GetSimpleData<DTO.Cliente>(@"SELECT TOP 10 [IDCLIENTE]
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
      ,[usuario_consulta]
      ,[fecha_consulta]
      ,[usuario_modificacion]
      ,[fecha_modificacion]
      ,replace(replace(replace(cast([CONTACTOS] as nvarchar),char(13),''),char(10),''),'""','') as [CONTACTOS]
      ,[CODFACTUSOL]
      ,[TIPO] FROM CLIENTES");

            }
        }

    }
}

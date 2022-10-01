﻿using DTO;
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
                Avisos = db.GetSimpleData<DTO.Aviso>(@"SELECT TOP 50
a.IDCASO,
a.FECHA,
a.CLIENTE 'IDCLIENTE',
cli.NOMBRE 'CLIENTE',
e.DESCRIPCION 'ESTADO',
t.DESCRIPCION 'TIPO',
o.DESCRIPCION 'ORIGEN',
STRING_ESCAPE(cast(a.DESCRIPCION as nvarchar),'json') 'DESCRIPCION',
a.Prioridad,
a.NUMCASO,
f.DESCRIPCION 'FUENTE',
cast(ISNULL(ac.IdCaso, 0) as bit) 'ASIGNADO',
emp.EmployeeID,
emp.FirstName + ' ' + emp.LastName 'EMPLEADO'
FROM CASOS a
LEFT JOIN CLIENTES cli ON a.CLIENTE = cli.IDCLIENTE 
JOIN ESTADOS e On e.IDESTADO = a.ESTADO
JOIN TIPOS t ON t.IDTIPO = a.TIPO
JOIN ORIGEN o ON o.IDORIGEN = a.ORIGEN
JOIN FUENTES f ON f.IdFuente = a.FUENTE
LEFT JOIN AsignacionCasos ac ON ac.IdCaso = a.IDCASO
LEFT JOIN Empleados emp ON emp.EmployeeID = ac.EmployeeID", null).ToList();

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
      ,STRING_ESCAPE(cast([CONTACTOS] as nvarchar),'json') as [CONTACTOS]
      ,[CODFACTUSOL]
      ,[TIPO] FROM CLIENTES");

            }
        }

    }
}

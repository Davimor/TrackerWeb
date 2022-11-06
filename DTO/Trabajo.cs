using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Trabajo
    {
        public int id { get; set; }
        public string descripcion { get; set; } ="";
        public string estadoTrabajo { get; set; }
        public string tecnico { get; set; } = "";
        public string tipoTrabajo { get; set; } = "";
    }
}

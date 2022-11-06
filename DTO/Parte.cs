using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Parte
    {
        public string numParte { get; set; }
        public long idCliente { get; set; }
        public DateTime fecha { get; set; }
        public string observaciones { get; set; } = "";
        public string tipoIntervencion { get; set; }
        public List<Trabajo> trabajos { get; set; } = new List<Trabajo>();
    }
}

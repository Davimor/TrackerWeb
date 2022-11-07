using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Parte
    {
        public string IDINTERVENCION { get; set; }
        public string numParte { get; set; }
        public string idCliente { get; set; }
        public DateTime fecha { get; set; }
        public string observaciones { get; set; } = "";
        public string tipoIntervencion { get; set; }
        public string idUser { get; set; } = "";
        public List<Trabajo> trabajos { get; set; } = new List<Trabajo>();
        public List<Documento> docs { get; set; } = new List<Documento>();
    }
}

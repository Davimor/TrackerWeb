using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class HistorialAviso
    {
        public int CASO { get; set; }
        public DateTime? FECHA { get; set; }
        public string COMENTARIO { get; set; } = "";
        public string USUARIO { get; set; } = "";
    }
}

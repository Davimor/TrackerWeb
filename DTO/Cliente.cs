using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Cliente
    {
        public int IDCLIENTE { get; set; }

        public string NOMBRE { get; set; } = "";

        public string APELLIDOS { get; set; } = "";

        public string DIRECCION { get; set; } = "";

        public string POBLACION { get; set; } = "";

        public string CODPOSTAL { get; set; } = "";

        public int? PROVINCIA { get; set; }

        public string TELFIJO { get; set; } = "";

        public string TELMOVIL { get; set; } = "";

        public string EMAIL { get; set; } = "";

        public int? CARGO { get; set; }
        public string DESCARGO { get; set; } = "";

        public string usuario_modificacion { get; set; } = "";

        public DateTime? fecha_modificacion { get; set; } = DateTime.Now;

        public string Contactos { get; set; } = "";

        public int? CODFACTUSOL { get; set; }

        public int? TIPO { get; set; }

        public string DESTIPO { get; set; } = "";

    }
}

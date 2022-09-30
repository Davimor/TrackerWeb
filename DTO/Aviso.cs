namespace DTO
{
    public class Aviso
    {
        public int IDCASO { get; set; }

        public string NUMCASO { get; set; } = "";

        public int? CLIENTE { get; set; }

        public int? CONTACTO { get; set; }

        public int? ESTADO { get; set; }

        public int? TIPO { get; set; }

        public int? ORIGEN { get; set; }

        public int? FUENTE { get; set; }

        public string Prioridad { get; set; } = "";

        public DateTime? FECHA { get; set; }

        public string DESCRIPCION { get; set; } = "";

        public string usuario_consulta { get; set; } = "";

        public DateTime? fecha_consulta { get; set; }

        public string usuario_modificacion { get; set; } = "";

        public DateTime? fecha_modificacion { get; set; }
    }
}
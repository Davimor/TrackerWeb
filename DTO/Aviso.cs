namespace DTO
{
    public class Aviso
    {
        public int? IDCASO { get; set; }
        public DateTime? FECHA { get; set; }
        public int? IDCLIENTE { get; set; }
        public string CLIENTE { get; set; } = "";
        public string ESTADO { get; set; } = "";
        public string TIPO { get; set; } = "";
        public string ORIGEN { get; set; } = "";
        public string DESCRIPCION { get; set; } = "";
        public string PRIORIDAD { get; set; } = "";
        public string NUMCASO { get; set; } = "";
        public string FUENTE { get; set; } = "";
        public bool ASIGNADO { get; set; }
        public string EmployeeID { get; set; } = "";
        public string EMPLEADO { get; set; } = "";
        public string usuario_modificacion { get; set; } = "";
        public DateTime? fecha_modificacion { get; set; }
        public string DESESTADO { get; set; } = "";
        public string DESTIPO { get; set; } = "";
        public string DESORIGEN { get; set; } = "";
        public string DESFUENTE { get; set; } = "";
        public List<Documento> Documentos { get; set; } = new List<Documento>();

    }
}
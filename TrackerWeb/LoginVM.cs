namespace TrackerWeb
{
    public class LoginVM
    {

        public string IdUser { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string pass { get; set; }
        public bool Administrador { get; set; }
        public bool AsignaCasos { get; set; }

        public bool isRemember { get; set; }

    }
}

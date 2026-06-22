namespace SistemaBiblioteca {
    public class TipoSocio {
        public int ID {get; set;}
        public required string NombreTipo {get; set;}
        public int MaxSimultaneos {get; set;}
        public int DiasPrestamo {get; set;}
        public int MultaXDia {get; set;}
    }
};

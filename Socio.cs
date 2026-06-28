namespace SistemaBiblioteca {
    public class Socio {
        public required int NroSocio {get; set;}
        public required string Nombre {get; set;}
        public required string Apellido {get; set;}
        public required string Email {get; set;}
        public required int TipoId {get; set;}
        public required TipoSocio Tipo {get; set;}
        public required bool Activo {get; set;}
    }
}

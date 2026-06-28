namespace SistemaBiblioteca {
    public class Reserva {
        public required int SocioId {get; set;}
        public required Socio Socio {get; set;}

        public required string LibroISBN {get; set;}
        public required Libro Libro {get; set;}

        public required string FechaReserva {get; set;}

        public required int EstadoId {get; set;}
        public required EstadoReserva Estado {get; set;}
    }
}

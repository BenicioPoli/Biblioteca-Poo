namespace SistemaBiblioteca {
    public class Prestamo {
        public required int SocioId {get; set;}
        public required Socio Socio {get; set;}

        public required string LibroISBN {get; set;}
        public required Libro? Libro {get; set;}

        public required string FechaPrestamo {get; set;}
        public required string FechaVencimiento {get; set;}
        public string? FechaDevolucion {get; set;}

        public required int EstadoId {get; set;}
        public required EstadoPrestamo? Estado {get; set;}
    }
}

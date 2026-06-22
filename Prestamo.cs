namespace SistemaBiblioteca {
    public class Prestamo {
        public required Socio Socio {get; set;}
        public required Libro Libro {get; set;}
        public required string FechaPrestamo {get; set;}
        public required string FechaVencimiento {get; set;}
        public required string FechaDevolucion {get; set;}
        public required EstadoPrestamo estado {get; set;}
    }
}

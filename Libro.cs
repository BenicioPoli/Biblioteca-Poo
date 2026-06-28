namespace SistemaBiblioteca {
    public class Libro {
        public required string ISBN {get; set;}
        public required string Titulo {get; set;}
        public required string Autor {get; set;}
        public required int GeneroId {get; set;}
        public required Genero Genero {get; set;}
        public int CantidadCopias {get; set;}
    }
}

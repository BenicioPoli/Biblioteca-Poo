namespace SistemaBiblioteca {
    public class Libro {
        public required string ISBN {get; set;}
        public required string Titulo {get; set;}
        public required string Autor {get; set;}

	public ICollection<Genero> Generos {get; set;} = new List<Genero>();

        public int CantidadCopias {get; set;}
    }
}

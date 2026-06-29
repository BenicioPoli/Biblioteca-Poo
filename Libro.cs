namespace SistemaBiblioteca {
	public class Libro {
		public required string ISBN {get; set;}
		public required string Titulo {get; set;}
		public required string Autor {get; set;}

		public ICollection<Genero> Generos {get; set;} = new List<Genero>();

		public int CantidadCopias {get; set;}

		public void ImprimirDetalle() {
			string generosStr = Generos.Any() 
				? string.Join(", ", Generos.Select(g => g.Descripcion.Trim())) 
				: "(ninguno)";

			Console.WriteLine($"Título: {Titulo}");
			Console.WriteLine($"Autor: {Autor}");
			Console.WriteLine($"ISBN: {ISBN}");
			Console.WriteLine($"Géneros: {generosStr}");
			Console.WriteLine($"Copias Disponibles: {CantidadCopias}");
		}
	}
}

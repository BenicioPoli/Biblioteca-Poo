using Microsoft.EntityFrameworkCore;

namespace SistemaBiblioteca
{
	internal class Program
	{
		static void Main(string[] args)
		{
			GestorBiblioteca gestor = new GestorBiblioteca();

			Console.WriteLine("Ingrese el numero de socio: ");
			int nroSocio = int.Parse(Console.ReadLine()!);

			var socio1 = gestor.BusquedaSocio(nroSocio);

			if (socio1 == null) {
				Console.WriteLine("Socio no encontrado.");
				return;
			}

			if (socio1.PuedePrestamo()) {
				gestor.HacerPrestamo(socio1);
			}

		}
	}
}

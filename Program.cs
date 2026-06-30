using Microsoft.EntityFrameworkCore;

namespace SistemaBiblioteca
{
	internal class Program
	{
		private static GestorBiblioteca gestor = new GestorBiblioteca();

		static void Main(string[] args) {

			var libros = gestor.VerStubLibros();
			Console.WriteLine($"{libros.Count} libros disponibles.");
			foreach (Libro libro in libros) {
				Console.WriteLine($"* {libro.Titulo} ({libro.Autor})");
			}

			var obtenerSocio = (string input) => {
				if (!int.TryParse(input, out int nroSocio)) {
					return null;
				}

				return gestor.BusquedaSocio(nroSocio);
			};

			var commands = new List<(string Description, Action Callback)>{
				("Ingresar N° de socio", () => {
					Socio s = Menu.ReadValid<Socio>("Ingrese su número de socio", obtenerSocio, "Socio no encontrado.");
					ManejoSocio(s);
				 }),
				("Ver consultas y reportes", VerReportes),
			};

			Menu.RunCommand("Sistema biblioteca", commands);
		}

		static void ManejoSocio(Socio socio) {
			var obtenerLibro = (string input) => {
				if (string.IsNullOrWhiteSpace(input)) return null;
				return gestor.BusquedaLibroPorTermino(input);
			};

			var nuevaReserva = () => {
				if (!socio.Activo) {
					Console.WriteLine("El socio no está activo, por lo que no puede realizar reservas.");
					return;
				}

				Libro libro = Menu.ReadValid<Libro>("Ingrese título o autor del libro a reservar", obtenerLibro, "No se encontró ningún libro con ese término.");

				Console.WriteLine();
				Console.WriteLine("Libro encontrado:");
				libro.ImprimirDetalle();
				Console.WriteLine();

				if (Menu.ConfirmPositive("¿Confirmar la reserva de este libro?")) {
					gestor.HacerReserva(socio, libro);
				}
			};

			var realizarPrestamo = () => {
				if (!socio.PuedePrestamo()) {
					Console.WriteLine("No puedes hacer préstamos actualmente.");
					return;
				}

				Libro libro = Menu.ReadValid<Libro>("Ingrese título o autor del libro a devolver", obtenerLibro, "No se encontró ningún libro con ese término.");

				Console.WriteLine();
				Console.WriteLine("Libro encontrado:");
				libro.ImprimirDetalle();
				Console.WriteLine();

				if (libro.CantidadCopias <= 0) {
					Console.WriteLine("El libro no tiene copias disponibles.");
					if (Menu.ConfirmPositive("¿Desea ir al flujo de reservas?")) {
						nuevaReserva();
					}

					return;
				}

				if (Menu.ConfirmPositive("¿Confirmar el préstamo de este libro?")) {
					gestor.HacerPrestamo(socio, libro);
				}
			};

			var realizarDevolucion = () => {
				Libro libro = Menu.ReadValid<Libro>("Ingrese título o autor del libro a devolver", obtenerLibro, "No se encontró ningún libro con ese término.");

				Console.WriteLine();
				Console.WriteLine("Libro encontrado:");
				libro.ImprimirDetalle();
				Console.WriteLine();

				if (Menu.ConfirmPositive("¿Confirmar devolución de este libro?")) {
					gestor.HacerDevolucion(socio, libro);
				}
			};

			var verDetalle = () => {
				Console.WriteLine();
				socio.ImprimirDetalle();
				Console.WriteLine();
			};

			var commands = new List<(string Description, Action Callback)>{
				("Realizar préstamo", realizarPrestamo),
				("Realizar devolución", realizarDevolucion),
				("Nueva reserva", nuevaReserva),
				("Ver detalle", verDetalle),
			};

			Menu.RunCommand($"Panel de socio: {socio.Nombre} {socio.Apellido}", commands);
		}

		static void VerReportes() {
			var commands = new List<(string Description, Action Callback)>{
				("Top 5 libros más prestados", gestor.LibrosMasPrestados),
				("Listado de socios con multas pendientes", gestor.SociosConMultasPendientes),
			};

			Menu.RunCommand("Sección Reportes Administrativos", commands);
		}
	}
}

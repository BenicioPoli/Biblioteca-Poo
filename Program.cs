using Microsoft.EntityFrameworkCore;

namespace SistemaBiblioteca
{
	internal class Program
	{
		static void Main(string[] args)
		{
			BibliotecaContext context = new BibliotecaContext();
			var socio = context.Socios
				.Include(s => s.Tipo)
				.ToList();
			Console.WriteLine("Ingrese el numero de socio: ");

			int nroSocio = int.Parse(Console.ReadLine()!);
			var socio1 = socio.FirstOrDefault(s => s.NroSocio == nroSocio);
			if (socio1 == null) {
				Console.WriteLine("Socio no encontrado.");
				return;
			}

			if (PuedePrestamo(socio1, context)) {
				HacerPrestamo(socio1, context);
			}

		}

		public static bool PuedePrestamo(Socio socio, BibliotecaContext context) {
			if (!socio.Activo) {
				Console.WriteLine("El socio no esta activo,por lo que no puede realizar un prestamo");
				return false;
			}

			var prestamosDelSocio = context.Prestamos
				.Include(p => p.Socio)
				.Where(p => p.SocioId == socio.NroSocio)
				.ToList();

			var prestamosVencidos = prestamosDelSocio
				.Where(p => p.Estado.Descripcion == "Vencido")
				.ToList();

			if (prestamosVencidos.Count > 0) {
				Console.WriteLine("El socio tiene multas pendientes, por lo que no puede realizar un nuevo préstamo.");
				return false;
			}

			var cantLibrosPuedeSocio = socio.Tipo.MaxSimultaneos;
			var prestamosActivos = prestamosDelSocio
				.Where(p => p.Estado.Descripcion == "Activo")
				.ToList();

			if (prestamosActivos.Count >= cantLibrosPuedeSocio)
			{
				Console.WriteLine("El socio supero su limite no puede realizar mas prestamos hasta que finalize alguno");
				return false;
			}
			return true;

		}

		public static void HacerPrestamo(Socio socio, BibliotecaContext context) {
			Console.WriteLine("Ingrese titulo o autor del Libro que desea retirar: ");
			string libro = Console.ReadLine()!;
			Libro libroEncontrado = context.Libros
				.Where(l => l.Titulo.Contains(libro) || l.Autor.Contains(libro))
				.FirstOrDefault()!;

			if (libroEncontrado == null) {
				Console.WriteLine("No se encontro el libro solicitado,vuelva a intentarlo");
				HacerPrestamo(socio, context);
				return;
			}

			if (libroEncontrado.CantidadCopias <= 0) {
				Console.WriteLine("El libro solicitado no esta disponible,desea reservarlo? (s/n)");
				string respuesta = Console.ReadLine()!;
				if (respuesta == "s") {
					HacerReserva(socio, context, libroEncontrado);
					return;
				} else {
					return;
				}
			}

			DateOnly fechaPrestamo = DateOnly.FromDateTime(DateTime.Now);
			DateOnly fechaVencimiento = fechaPrestamo.AddDays(socio.Tipo?.DiasPrestamo ?? 7);

			EstadoPrestamo estado = context.EstadosPrestamo
				.Where(e => e.Descripcion.Contains("Activo"))
				.FirstOrDefault()!;

			Prestamo nuevoPrestamo = new Prestamo {
				SocioId = socio.NroSocio,
				Socio = socio,
				LibroISBN = libroEncontrado.ISBN,
				Libro = libroEncontrado,
				EstadoId = estado.idEstado, 
				Estado = estado,

				FechaPrestamo = fechaPrestamo.ToString(),
				FechaDevolucion = null,
				FechaVencimiento = fechaVencimiento.ToString()
			};

			context.Prestamos.Add(nuevoPrestamo);
			libroEncontrado.CantidadCopias -= 1;
			context.SaveChanges();
			Console.WriteLine("Prestamo Registrado con exito");

		}

		public static void HacerReserva(Socio socio, BibliotecaContext context, Libro libro) {
			Reserva? reservaExistente = context.Reservas
				.Include(r => r.Socio)
				.Include(r => r.Libro)
				.Include(r => r.Estado)
				.Where(r => r.Socio.NroSocio == socio.NroSocio)
				.Where(r => r.Estado.Descripcion == "Pendiente")
				.Where(r => r.Libro.ISBN == libro.ISBN)
				.FirstOrDefault();

			if (reservaExistente != null) {
				Console.WriteLine("Ya tiene una reserva pendiente para este libro.");
				return;
			}

			DateOnly fechaReserva = DateOnly.FromDateTime(DateTime.Now);
			var estado = context.EstadosReserva
				.Where(e => e.Descripcion == "Pendiente")
				.FirstOrDefault();
			Reserva nuevareserva = new Reserva {
				SocioId = socio.NroSocio,
				Socio = socio,
				LibroISBN = libro.ISBN,
				Libro = libro,
				EstadoId = estado.idEstado,
				Estado = estado!,
				FechaReserva = fechaReserva.ToString()
			};

			context.Reservas.Add(nuevareserva);
			context.SaveChanges();
			Console.WriteLine("Reserva Registrada con exito");
		}
	}
}

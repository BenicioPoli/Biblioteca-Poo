using Biblioteca;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca
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

            int nroSocio = int.Parse(Console.ReadLine());
            var socio1 = socio.FirstOrDefault(s => s.NroSocio == nroSocio);
            if (socio1 == null)
            {
                Console.WriteLine("Socio no encontrado.");
                return;
            }
            if (PuedePrestamo(socio1, context))
            {
                HacerPrestamo(socio1, context);
            }


        }
    }
    public bool PuedePrestamo(Socio socio, BibliotecaContext context) {
            if (socio.Activo == 0)
            {
                Console.WriteLine("El socio no esta activo,por lo que no puede realizar un prestamo");
                return false;
            }
            var prestamosDelSocio = context.Prestamos
                               .Where(p => p.IdSocio == socio.NroSocio)
                               .Include(p => p.Estado)
                               .ToList();

            var prestamosVencidos = prestamosDelSocio
                               .Where(p => p.Estado.Descripcion == "Vencido")
                               .ToList();

            if (prestamosVencidos.Count > 0)
            {
                Console.WriteLine("El socio tiene multas pendientes, por lo que no puede realizar un nuevo préstamo.");
                return false;
            }

            var cantLibrosPuedeSocio = socio.Tipo.MaxLibros;
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




        public void HacerPrestamo(Socio socio, BibliotecaContext context)
        {
            Console.WriteLine("Ingrese titulo o autor del Libro que desea retirar: ");
            string libro = Console.ReadLine();
            Libro libroEncontrado = context.Libros
                                .Where(l => l.Titulo.Contains(libro) || l.Autor.Contains(libro))
                                .FirstOrDefault();
            if (libroEncontrado == null)
            {
                Console.WriteLine("No se encontro el libro solicitado,vuelva a intentarlo");
                return HacerPrestamo(socio, context);
            }
            if (libroEncontrado.CantidadDisponible <= 0)
            {
                Console.WriteLine("El libro solicitado no esta disponible,desea reservarlo? (s/n)");
                string respuesta = Console.ReadLine();
                if (respuesta == "s")
                {
                    HacerReserva(socio, context, libroEncontrado);
                    return;
                }
                else
                {
                    return;
                }
            }
            
            DateOnly FechaPrestamo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly FechaDevolucion = FechaPrestamo.AddDays(socio.Tipo.DiasPrestamo);
            int idEstado = context.EstadoPrestamo
                            .Where(e => e.Descripcion == "Activo")
                            .Select(e => e.IdEstado)
                            .FirstOrDefault();
            Prestamo nuevoPrestamo = new Prestamo(socio.NroSocio, libroEncontrado.Isbn, FechaPrestamo,FechaDevolucion,null,idEstado);
            context.Prestamos.Add(nuevoPrestamo);
            libroEncontrado.cantidadDisponible -= 1;
            context.SaveChanges();
            Console.WriteLine("Prestamo Registrado con exito");

        }
        public void HacerReserva(Socio socio, BibliotecaContext context, Libro libro)
        {
            Reserva reservaExistente = context.Reserva.FirstOrDefault(r => r.IdSocio == socio.NroSocio && r.Isbn == libro.Isbn && r.Estado.Descripcion == "Pendiente");
            if (reservaExistente != null)
            {
                Console.WriteLine("Ya tiene una reserva pendiente para este libro.");
                return;
            }
            DateOnly FechaReserva = DateOnly.FromDateTime(DateTime.Now);
            int idEstado = context.EstadoReserva
                           .Where(e => e.Descripcion == "Pendiente")
                           .Select(e => e.IdEstado)
                           .FirstOrDefault();
            Reserva nuevareserva = new Reserva(socio.NroSocio, libro.Isbn, FechaReserva, idEstado);
            context.Reservas.Add(nuevareserva);
            context.SaveChanges();
            Console.WriteLine("Reserva Registrada con exito");
        }
    }
}

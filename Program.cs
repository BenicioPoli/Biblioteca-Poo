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



        }
    }
    public bool PuedePrestamo(Socio socio,var context)
        {
            if (socio.Activo == 0)
            {
                Console.WriteLine("El socio no esta activo,por lo que no puede realizar un prestamo");
                return false;
            }
            var prestamosDelSocio = context.Prestamos
                               .Where(p => p.IdSocio == idDelSocio)
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
    }
}

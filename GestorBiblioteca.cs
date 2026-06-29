using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SistemaBiblioteca
{
    public class GestorBiblioteca
    {
        private BibliotecaContext context;

        public GestorBiblioteca()
        {
            this.context = new BibliotecaContext();
        }

        public Socio BusquedaSocio(int nroSocio)
        {
            var socio = context.Socios
                .Include(s => s.Tipo)
                .FirstOrDefault(s => s.NroSocio == nroSocio);
            if (socio == null)
            {
                Console.WriteLine("Socio no encontrado.");
                return null;
            }
            return socio;
        }

        public List<Prestamo> BusquedaPrestamos(Socio socio)
        {
            var prestamos = context.Prestamos
                .Include(p => p.Socio)
                .Include(p => p.Estado)
                .Where(p => p.SocioId == socio.NroSocio)
                .ToList();
            return prestamos;
        }

        public void HacerPrestamo(Socio socio)
        {
            Console.WriteLine("Ingrese titulo o autor del Libro que desea retirar: ");
            string libro = Console.ReadLine()!;
            Libro libroEncontrado = context.Libros
                .Where(l => l.Titulo.Contains(libro) || l.Autor.Contains(libro))
                .FirstOrDefault()!;

            if (libroEncontrado == null)
            {
                Console.WriteLine("No se encontro el libro solicitado,vuelva a intentarlo");
                HacerPrestamo(socio);
                return;
            }

            if (libroEncontrado.CantidadCopias <= 0)
            {
                Console.WriteLine("El libro solicitado no esta disponible,desea reservarlo? (s/n)");
                string respuesta = Console.ReadLine()!;
                if (respuesta == "s")
                {
                    HacerReserva(socio, libroEncontrado);
                    return;
                }
                else
                {
                    return;
                }
            }

            DateOnly fechaPrestamo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly fechaVencimiento = fechaPrestamo.AddDays(socio.Tipo?.DiasPrestamo ?? 7);

            EstadoPrestamo estado = context.EstadosPrestamo
                .Where(e => e.Descripcion.Contains("Activo"))
                .FirstOrDefault()!;

            Prestamo nuevoPrestamo = new Prestamo(socio.NroSocio, libroEncontrado.ISBN, fechaPrestamo.ToString(), fechaVencimiento.ToString(), estado.idEstado);

            context.Prestamos.Add(nuevoPrestamo);
            libroEncontrado.CantidadCopias -= 1;
            context.SaveChanges();
            Console.WriteLine("Prestamo Registrado con exito");

        }

        public void HacerReserva(Socio socio, Libro libro)
        {
            Reserva? reservaExistente = context.Reservas
                .Include(r => r.Socio)
                .Include(r => r.Libro)
                .Include(r => r.Estado)
                .Where(r => r.Socio.NroSocio == socio.NroSocio)
                .Where(r => r.Estado.Descripcion == "Pendiente")
                .Where(r => r.Libro.ISBN == libro.ISBN)
                .FirstOrDefault();

            if (reservaExistente != null)
            {
                Console.WriteLine("Ya tiene una reserva pendiente para este libro.");
                return;
            }

            DateOnly fechaReserva = DateOnly.FromDateTime(DateTime.Now);
            var estado = context.EstadosReserva
                .Where(e => e.Descripcion == "Pendiente")
                .FirstOrDefault();

            Reserva nuevareserva = new Reserva(socio.NroSocio, libro.ISBN, fechaReserva.ToString(), estado!.idEstado);

            context.Reservas.Add(nuevareserva);
            context.SaveChanges();
            Console.WriteLine("Reserva Registrada con exito");
        }

        public void HacerDevolucion(Socio socio,Libro libro){

           var prestamoActivo = context.Prestamos
           .Include(p => p.Estado)
           .Where(p => p.Socio.NroSocio == socio.NroSocio)
           .Where(p => p.Libro.ISBN == libro.ISBN)
           .Where(p => p.Estado.Descripcion == "Activo")
           .FirstOrDefault(); 

           if(prestamoActivo == null){
               Console.WriteLine("No tiene un prestamo activo con este libro");
               return;
           }
           
           DateOnly fechaDevolucion = DateOnly.FromDateTime(DateTime.Now);
           DateOnly fechaVencimiento = DateOnly.Parse(prestamoActivo.FechaVencimiento);
           prestamoActivo.FechaDevolucion = fechaDevolucion.ToString();

           if (fechaDevolucion > fechaVencimiento){
                prestamoActivo.Multa = socio.Tipo.MultaXDia * (fechaDevolucion.DayNumber - fechaVencimiento.DayNumber);
                Console.WriteLine("Entrego el libro tarde va a tener que pagar una multa de: " + prestamoActivo.Multa);
                prestamoActivo.EstadoId = context.EstadosPrestamo
                    .Where(e => e.Descripcion == "Vencido")
                    .FirstOrDefault()!.idEstado;
           }
           else
           {
                Console.WriteLine("Entrego el libro a tiempo, no tiene que pagar multa");   
                prestamoActivo.EstadoId = context.EstadosPrestamo
                    .Where(e => e.Descripcion == "Devuelto")
                    .FirstOrDefault()!.idEstado;
           }
           
           var ReservaLibro = context.Reservas
                .Include(r => r.Estado)
                .Where(r => r.Libro.ISBN == libro.ISBN)
                .Where(r => r.Estado.Descripcion == "Pendiente")
                .ToList()
                .OrderBy(r => DateOnly.Parse(r.FechaReserva))
                .FirstOrDefault();
        
            if (ReservaLibro != null)
            {
                ReservaLibro.EstadoId = context.EstadosReserva
                    .Where(e => e.Descripcion == "Cumplida")
                    .FirstOrDefault()!.idEstado;
                Console.WriteLine("La reserva mas antigua del libro paso a Cumplida");
            }
            else
            {
                libro.CantidadCopias += 1;
                Console.WriteLine("No habia reservas asi que el libro paso a estar disponible para prestamo");
            }

            context.SaveChanges();
        }

        public void LibrosMasPrestados()
        {
            var librosMasPrestados = context.Prestamos
                .GroupBy(p => p.LibroISBN)
                .Select(g => new { ISBN = g.Key, CantidadPrestamos = g.Count() })
                .OrderByDescending(x => x.CantidadPrestamos)
                .Take(5)
                .ToList();

            Console.WriteLine("Los 5 libros más prestados son:");
            foreach (var libro in librosMasPrestados)
            {
                var libroInfo = context.Libros.FirstOrDefault(l => l.ISBN == libro.ISBN);
                if (libroInfo != null)
                {
                    Console.WriteLine($"- {libroInfo.Titulo} por {libroInfo.Autor}, ISBN: {libroInfo.ISBN}, Cantidad de Prestamos: {libro.CantidadPrestamos}");
                }
            }
        }

        public void SociosConMultasPendientes()
        {
            var sociosConMultas = context.Prestamos
                .Where(p => p.Multa != null)
                .Where(p => p.Estado.Descripcion == "Vencido")
                .GroupBy(p => p.SocioId)
                .Select(g => new { NroSocio = g.Key, TotalMultas = g.Sum(p => p.Multa) })
                .ToList();

            Console.WriteLine("Socios con multas pendientes:");

            foreach (var socio in sociosConMultas)
            {
                var socioInfo = context.Socios.FirstOrDefault(s => s.NroSocio == socio.NroSocio);
                if (socioInfo != null)
                {
                    Console.WriteLine($"- {socioInfo.Nombre} {socioInfo.Apellido}, Nro Socio: {socioInfo.NroSocio}, Total Multas Pendientes: {socio.TotalMultas}");
                }
            }
        }
    }

}

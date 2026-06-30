namespace SistemaBiblioteca {
    public class Socio {
        public required int NroSocio {get; set;}
        public required string Nombre {get; set;}
        public required string Apellido {get; set;}
        public required string Email {get; set;}
        public required int TipoId {get; set;}
        public required TipoSocio Tipo {get; set;}
        public required bool Activo {get; set;}

	public ICollection<Prestamo> Prestamos {get; set;} = new List<Prestamo>();
	public ICollection<Reserva> Reservas {get; set;} = new List<Reserva>();

        public bool PuedePrestamo()
        {
            if (!this.Activo)
            {
                Console.WriteLine("El socio no esta activo,por lo que no puede realizar un prestamo");
                return false;
            }

            var gestor = new GestorBiblioteca();
            var multasPendientes = Prestamos
                .Where(p => p.Multa != null)
                .ToList();

            if (multasPendientes.Count > 0)
            {
                Console.WriteLine("El socio tiene multas pendientes, por lo que no puede realizar un nuevo préstamo.");
                return false;
            }

            var cantLibrosPuedeSocio = Tipo.MaxSimultaneos;
            var prestamosActivos = Prestamos
                .Where(p => p.Estado.Descripcion.Contains("Activo"))
                .ToList();

            if (prestamosActivos.Count >= cantLibrosPuedeSocio)
            {
                Console.WriteLine("El socio supero su limite no puede realizar mas prestamos hasta que finalize alguno");
                return false;
            }
            return true;

        }

        public void ImprimirDetalle() {
            Console.WriteLine($"Socio: {Nombre} {Apellido}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Tipo de Socio: {Tipo.NombreTipo}");
            Console.WriteLine($"Activo: {(Activo ? "Sí" : "No")}");
            Console.WriteLine("Prestamos Activos:");
            var prestamosActivos = Prestamos.Where(p => p.Estado.Descripcion.Contains("Activo")).ToList();

            foreach (var prestamo in prestamosActivos)
            {
                Console.WriteLine($"- Libro ISBN: {prestamo.LibroISBN}, Estado: {prestamo.Estado.Descripcion}, Fecha Prestamo: {prestamo.FechaPrestamo}, Fecha Vencimiento: {prestamo.FechaVencimiento}, Fecha Devolucion: {prestamo.FechaDevolucion ?? "No devuelto"}, Multa: {(prestamo.Multa.HasValue ? prestamo.Multa.Value.ToString() : "No aplica")}");
            }

            Console.WriteLine("Reservas:");
            foreach (var reserva in Reservas)
            {
                Console.WriteLine($"- Libro: {reserva.Libro.Titulo}, Estado: {reserva.Estado.Descripcion}, Fecha Reserva: {reserva.FechaReserva}");
            }
	    
            Console.WriteLine("Historial de Devoluciones:");
            var devoluciones = Prestamos.Where(p => p.Estado.Descripcion.Contains("Devuelto")).ToList();
            foreach (var devolucion in devoluciones)
            {
                Console.WriteLine($"- Libro ISBN: {devolucion.LibroISBN}, Fecha Devolucion: {devolucion.FechaDevolucion}, Multa: {(devolucion.Multa.HasValue ? devolucion.Multa.Value.ToString() : "No aplica")}");
            }

            Console.WriteLine("Prestamos con Multas Pendientes:");
            var multasPendientes = Prestamos.Where(p => p.Estado.Descripcion.Contains("Vencido")).ToList();
            foreach (var prestamo in multasPendientes)
            {
                Console.WriteLine($"- Libro ISBN: {prestamo.LibroISBN}, Fecha Vencimiento: {prestamo.FechaVencimiento}, Multa: {prestamo.Multa.Value}");
            }
        }

    }
}

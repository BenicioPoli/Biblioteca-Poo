namespace SistemaBiblioteca {
    public class Socio {
        public required int NroSocio {get; set;}
        public required string Nombre {get; set;}
        public required string Apellido {get; set;}
        public required string Email {get; set;}
        public required int TipoId {get; set;}
        public required TipoSocio Tipo {get; set;}
        public required bool Activo {get; set;}

        public bool PuedePrestamo()
        {
            if (!this.Activo)
            {
                Console.WriteLine("El socio no esta activo,por lo que no puede realizar un prestamo");
                return false;
            }

            var gestor = new GestorBiblioteca();
            var prestamosDelSocio = gestor.BusquedaPrestamos(this);

            var multasPendientes = prestamosDelSocio
                .Where(p => p.Multa != null)
                .ToList();

            if (multasPendientes.Count > 0)
            {
                Console.WriteLine("El socio tiene multas pendientes, por lo que no puede realizar un nuevo préstamo.");
                return false;
            }

            var cantLibrosPuedeSocio = Tipo.MaxSimultaneos;
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

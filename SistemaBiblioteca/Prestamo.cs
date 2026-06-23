namespace SistemaBiblioteca {
    public class Prestamo {
        public Socio Socio;
        public Libro Libro;
        public string FechaVencimiento;
        public string FechaPrestamo;
        public string? FechaDevolucion;
        public EstadoPrestamo Estado;
        public int? Multa;
        public int NroSocio;
        public string isbn;
        public int idEstado;

        public Prestamo(int nroSocio,int isbn,string fechaPrestamo, string fechaVencimiento, int idEstado)
        {
            FechaVencimiento = fechaVencimiento;
            FechaPrestamo = fechaPrestamo;
            NroSocio = nroSocio;
            this.isbn = isbn;
            this.idEstado = idEstado;
        })
    }
}

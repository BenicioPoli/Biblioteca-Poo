namespace SistemaBiblioteca {
    public class Reserva {
        public int NroSocio;
        public int ISBN;
        public int IdEstado;
        public Socio Socio;
        public Libro Libro;
        public string FechaReserva;
        public EstadoReserva Estado;

        public Reserva(int nroSocio, int isbn, string fechaReserva, int idEstado)
        {
            NroSocio = nroSocio;
            ISBN = isbn;
            IdEstado = idEstado;
            FechaReserva = fechaReserva;
          
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaBiblioteca
{
    public class GestorBiblioteca
    {
        // private BibliotecaContext m_context;
        // con constructor y destructor etc.

        public void AgregarPrestamo(Prestamo prestamo) {
            //m_context.Prestamos.Add(prestamo);
            //m_context.SaveChanges();
        }

        public void RealizarDevolucion(Prestamo prestamo)
        {
        }

        public Socio BuscarSocio(int nroSocio)
        {
            return null;
        }

        public List<Reserva> ReservasSocio(Socio s)
        {
            return new List<Reserva>();
        }
    }
}

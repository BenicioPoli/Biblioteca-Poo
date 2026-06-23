namespace SistemaBiblioteca
{
    public class Program
    {
        //public static void RunCommand(string title, List<(string Description, Action Callback)> commands)
        
        public static void Main(string[] args)
        {
            Console.WriteLine("PINGA");

            var comandos = new List<(string Description, Action Callback)> {
                ("Hacer un préstamo", () => {return; }),
                ("Devolver un libro", () => {return; }),
                ("Reservar un libro", () => {return; }),
                ("Ver detalles del socio", () => {return; }),
            };
            Menu.RunCommand("Sistema biblioteca", comandos);
        }
    }
}
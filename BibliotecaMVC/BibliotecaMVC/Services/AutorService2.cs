using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorService2:IAutorService
    {
        private static List<Autor> autores = new List<Autor>
        {
            new Autor
            {
                ID = 1,
                Nombre = "Grace",
                Apellido = "Hopper",
                Nacionalidad = "Estadounidense",
                FechaNacimiento = new DateOnly(1906, 12, 9),
                Activo = false
            },
            new Autor
            {
                ID = 2,
                Nombre = "Alan",
                Apellido = "Turing",
                Nacionalidad = "Británico",
                FechaNacimiento = new DateOnly(1912, 6, 23),
                Activo = false
            },
            new Autor
            {
                ID = 3,
                Nombre = "Margaret",
                Apellido = "Hamilton",
                Nacionalidad = "Estadounidense",
                FechaNacimiento = new DateOnly(1936, 8, 17),
                Activo = true
            }
        };

        public Autor? ObtenerPorId(int id)
        {
            return autores.FirstOrDefault(x => x.ID == id);
        }

        public IEnumerable<Autor> ObtenerTodos()
        {
            return autores;
        }
    }
}

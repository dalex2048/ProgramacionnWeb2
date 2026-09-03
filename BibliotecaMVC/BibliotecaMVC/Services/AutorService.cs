using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorService : IAutorService
    {
        private static List<Autor> autores = new List<Autor>
        {
                new Autor
                {
                    ID = 1,
                    Nombre = "Dennis",
                    Apellido = "Ritchie",
                    Nacionalidad = "Estadounidense",
                    FechaNacimiento = new DateOnly(1941, 9, 9),
                    Activo = false
                },
                new Autor
                {
                    ID = 2,
                    Nombre = "Linus",
                    Apellido = "Torvalds",
                    Nacionalidad = "Finlandés",
                    FechaNacimiento = new DateOnly(1969, 12, 28),
                    Activo = true
                },
                new Autor
                {
                    ID = 3,
                    Nombre = "Ada",
                    Apellido = "Lovelace",
                    Nacionalidad = "Británica",
                    FechaNacimiento = new DateOnly(1815, 12, 10),
                    Activo = false
                },
                new Autor
                {
                    ID = 4,
                    Nombre = "Anders",
                    Apellido = "Hejlsberg",
                    Nacionalidad = "Danés",
                    FechaNacimiento = new DateOnly(1960, 12, 2),
                    Activo = true
                },
                new Autor
                {
                    ID = 5,
                    Nombre = "Martin",
                    Apellido = "Fowler",
                    Nacionalidad = "Británico",
                    FechaNacimiento = new DateOnly(1963, 12, 18),
                    Activo = true
                }
                ,
                new Autor
                {
                    ID = 6,
                    Nombre = "Martin2",
                    Apellido = "Fowler2",
                    Nacionalidad = "Británico2",
                    FechaNacimiento = new DateOnly(1963, 12, 18),
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

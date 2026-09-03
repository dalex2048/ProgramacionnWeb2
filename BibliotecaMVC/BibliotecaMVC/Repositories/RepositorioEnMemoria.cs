using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public class RepositorioEnMemoria : IRepositorioLibro
    {
        public IEnumerable<Libro> ObtenerTodos()
        {
            return new List<Libro>(){
            new Libro{ID = 1,Titulo = "Clean Code",Autor = "Robert Martin", Categoria = "Programación", Precio = 35.3M, Disponible = true},
            new Libro{ID = 2,Titulo = "Clean Arquitecture",Autor = "Carlos Martin", Categoria = "Programación", Precio = 25.9M, Disponible = false},
            new Libro{ID = 3,Titulo = "C# MVC",Autor = "Carlos Martin", Categoria = "Programación", Precio = 25.9M, Disponible = true}
        };
        }
    }
}

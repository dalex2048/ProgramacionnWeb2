using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        public IActionResult Index()
        {
            List<Libro> libros = new List<Libro>()
            {
                new Libro{ID = 1,Titulo = "Clean Code",Autor = "Robert Martin", Categoria = "Programación", Precio = 35.3M, Disponible = true},
                new Libro{ID = 2,Titulo = "Clean Arquitecture",Autor = "Carlos Martin", Categoria = "Programación", Precio = 25.9M, Disponible = false}
            };

            return View(libros);
        }
    }
}

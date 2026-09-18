using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        /*
        private readonly IRepositorioLibro _repositorio; 

        public LibrosController(IRepositorioLibro repositorio)
        {
            _repositorio = repositorio;
        }
        private static List<Libro> libros = new List<Libro>
        {
            new Libro{ID = 1,Titulo = "Clean Code",Autor = "Robert Martin", Categoria = "Programación", Precio = 35.3M, Disponible = true},
            new Libro{ID = 2,Titulo = "Clean Arquitecture",Autor = "Carlos Martin", Categoria = "Programación", Precio = 25.9M, Disponible = false},
            new Libro{ID = 3,Titulo = "C# MVC",Autor = "Carlos Martin", Categoria = "Programación", Precio = 25.9M, Disponible = true}
        };
        */
        private readonly BibliotecaContext _context;
        public LibrosController(BibliotecaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var listLibros = await _context.Libros.ToListAsync();
            return View(listLibros);
        }

        public async Task<IActionResult> Details(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if(libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        public IActionResult Create()
        {

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /*
        public IActionResult Edit(int id)
        {
            var libro = libros.FirstOrDefault(y => y.ID == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Libro libro)
        {
            if (id != libro.ID)
                return NotFound();
            if(!ModelState.IsValid)
                return View(libro) ;
            var _libro = libros.FirstOrDefault(x  => x.ID == id);
            if (_libro == null)
                return NotFound();

            _libro.Titulo = libro.Titulo;
            _libro.Autor = libro.Autor;
            _libro.Categoria = libro.Categoria;
            _libro.Precio = libro.Precio;
            _libro.Disponible = libro.Disponible;

            return RedirectToAction(nameof(Index));

        }


        public IActionResult Delete(int id)
        {
            var libro = libros.FirstOrDefault(x=> x.ID == id);
            if (libro == null)
                return NotFound();
            return View(libro);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            var libro = libros.FirstOrDefault(x=> x.ID ==id);
            if (libro == null)
                return NotFound();
            libros.Remove(libro);
            return RedirectToAction(nameof(Index));
        }
        */



    }
}

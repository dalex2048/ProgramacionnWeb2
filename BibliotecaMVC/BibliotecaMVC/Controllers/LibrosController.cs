using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaMVC.Controllers
{

    [Authorize(Roles = "Administrador, Bibliotecario, Usuario")]
    public class LibrosController : Controller
    {
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
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Create()
        {

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
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
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            var Libro = await _context.Libros.FindAsync(id);
            if(Libro == null)
            {
                return NotFound();
            }
            return View(Libro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(Libro libro, int id)
        {
            if(id != libro.ID)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            var exists = await _context.Libros.AnyAsync(a => a.ID == id);
            if (!exists)
            {
                return NotFound();
            }
            _context.Update(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if(libro == null)
            {
                return NotFound();
            }
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

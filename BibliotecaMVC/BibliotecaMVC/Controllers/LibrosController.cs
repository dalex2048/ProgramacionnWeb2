using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaMVC.Controllers
{

    [Authorize]
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<LibrosController> _logger;
        public LibrosController(BibliotecaContext context, ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando TODOS los libros");
            var listLibros = await _context.Libros.ToListAsync();
            return View(listLibros);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consultando los detalles del Libro con id {id}", id);
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el Libro con id {Id}", id);
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el detalle del Libro con {id}", id);
                throw;
            }

            
        }
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la creación de un Libro");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Create (Libro libro)
        {
            _logger.LogInformation("Creando un Libro");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El Libro enviado no cumple con los requerimientos minimos");
                return View(libro);
            }
            try
            {
                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al crear el Libro");
                throw;
            }
            
        }
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Iniciando la operación de edición del Libro con id {id}", id);
            try
            {
                var Libro = await _context.Libros.FindAsync(id);
                if (Libro == null)
                {
                    _logger.LogWarning("No se encontró el Libro con id {Id}", id);
                    return NotFound();
                }
                return View(Libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al iniciar la operación editar del Libro con id {id}", id);
                throw;
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(Libro libro, int id)
        {
            _logger.LogInformation("Editando el Libro con id {id}", id);
            if (id != libro.ID)
            {
                _logger.LogWarning("El Libro enviado no coincide con Id enviado");
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El Libro enviado no cumple con los requerimientos mínimos");
                return View(libro);
            }
            try
            {
                var exists = await _context.Libros.AnyAsync(a => a.ID == id);
                if (!exists)
                {
                    _logger.LogWarning("No se encontró el Libro con id {Id}", id);
                    return NotFound();
                }
                _context.Update(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar el Libro con id {id}", id);
                throw;
            }
            
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Eliminando Libro con id {id}", id);
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el Libro con id {Id}", id);
                    return NotFound();
                }
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el Libro con id {id}", id);
                throw;
            }
            
        }
    }
}

using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Elements;


namespace BibliotecaMVC.Controllers


{
    [Authorize]
    public class AutorController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<AutorController> _logger;

        public AutorController(BibliotecaContext context, ILogger<AutorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando el listado de autores");
            try
            {
                var autores = await _context.Autores.ToListAsync();
                return View(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar listado de autores");
                throw;
            }
            
            
        }
        public async Task<IActionResult> Reporte()
        {
            var autores = await _context.Autores.AsNoTracking().OrderBy(autor => autor.Nombre).ToListAsync();
            return File(GenerarReporte(autores), "application/pdf");
        }
        public async Task<IActionResult> DescargarReporte()
        {
            var autores = await _context.Autores.AsNoTracking().OrderBy(autor => autor.Nombre).ToListAsync();
            return File(GenerarReporte(autores), "application/pdf", "reporte-autores.pdf");
        }
        private static byte[] GenerarReporte(IReadOnlyCollection<Autor> autores)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            return Document.Create(document => document.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(42);
                page.PageColor(Colors.Grey.Lighten5);

                page.Header().Column(column =>
                {
                    column.Item().Text("BIBLIOTECA UNIVERSITARIA").FontSize(11).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Reporte de autores").FontSize(26).Bold().FontColor(Colors.Grey.Darken3);
                    column.Item().Text($"Listado actualizado al {DateTime.Now:dd/MM/yyyy}").FontSize(10).Bold().FontColor(Colors.Grey.Darken3);
                    column.Item().Padding(14).LineHorizontal(2).LineColor(Colors.Blue.Medium);
                });
                page.Content().PaddingVertical(22).Column(column =>
                {
                    column.Item().Background(Colors.Blue.Darken2).Padding(14).Row(row =>
                    {
                        row.RelativeItem().Text("Autores Registrados").FontSize(11).Bold().FontColor(Colors.White);
                        row.ConstantItem(45).AlignRight().Text(autores.Count.ToString()).FontSize(18).Bold().FontColor(Colors.White);

                    });
                    column.Item().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(42);
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.3f);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("ID");
                            header.Cell().Element(HeaderCellStyle).Text("NOMBRE");
                            header.Cell().Element(HeaderCellStyle).Text("NACIONALIDAD");
                            header.Cell().Element(HeaderCellStyle).Text("NACIMIENTO");
                        });
                        foreach(var autor in autores)
                        {
                            table.Cell().Element(BodyCellStyle).Text(autor.ID.ToString());
                            table.Cell().Element(BodyCellStyle).Text(autor.Nombre);
                            table.Cell().Element(BodyCellStyle).Text(autor.Nacionalidad ?? "No especificado");
                            table.Cell().Element(BodyCellStyle).Text(autor.FechaNacimiento.ToString("dd/MM/yyyy"));
                        }
                    });
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Biblioteca universitaria - Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            })).GeneratePdf();
            static IContainer HeaderCellStyle(IContainer container) => container.Background(Colors.Blue.Darken2).
                PaddingVertical(9).PaddingHorizontal(8).DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.White));
            static IContainer BodyCellStyle(IContainer container) => container.Background(Colors.White).BorderBottom(1).
                BorderColor(Colors.Grey.Lighten2).
                PaddingVertical(9).PaddingHorizontal(8).DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.Grey.Darken3));
        }
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Consultando los detalles del autor con id {id}");
            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    _logger.LogWarning("No se encontró el autor con id {id}", id);
                    return NotFound();
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Se produjo un error al consultar el detalle del autor {id}");
                throw;
            }
            
        }
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la creación de un autor");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> Create(Autor autor)
        {
            _logger.LogInformation("Creando un autor");

            if (!ModelState.IsValid)
            {
                _logger.LogError("El autor enviado no cumple con los requerimientos minimos");
                return View(autor);
            }
            try
            {
                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se´produjo un error al crear el autor");
                throw;
            }
            
        }
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Iniciando la operación de edición del autor con id {id}");
            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    return NotFound();
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Se produjo un error al iniciar la operación del editor con id {id}");
                throw;
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Edit(Autor autor, int id)
        {
            _logger.LogInformation($"Editando el autor con id {id}");
            if (id != autor.ID)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            try
            {
                var exists = await _context.Autores.AnyAsync(a => a.ID == id);

                if (!exists)
                {
                    _logger.LogWarning($"No se encontro el autor con id {id}");
                    return NotFound();
                }
                _context.Update(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Se produjo un error al editar el autor con id {id}");
                throw;
            }
            
            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Eliminando autor con id {id}");
            try
            {
                var autor = await _context.Autores.FindAsync(id);
                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor con el id {id}", id);
                    return NotFound();
                }
                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el autor con id {id}", id);
                throw;
            }
            
        }
        /*
        public IActionResult Delete(int id)
        {
            var autor = autores.FirstOrDefault(x =>x.ID == id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }
        [HttpPost,  ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            var autor = autores.FirstOrDefault(x =>x.ID == id);
            if(autor == null)
            {
                return NotFound();
            }
            autores.Remove(autor);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            var autor = autores.FirstOrDefault(y => y.ID == id);
            if (autor == null)
                return NotFound();
            return View(autor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Autor autor)
        {
            if (id != autor.ID)
                return NotFound();
            if (!ModelState.IsValid)
                return View(autor);
            var _autor = autores.FirstOrDefault(x =>x.ID == id);
            if (_autor == null)
                return NotFound();

            _autor.Nombre = autor.Nombre;
            _autor.Apellido = autor.Apellido;
            _autor.Nacionalidad = autor.Nacionalidad;
            _autor.FechaNacimiento = autor.FechaNacimiento;
            _autor.Activo = autor.Activo;
            return RedirectToAction(nameof(Index));
        }
        */

    }
}

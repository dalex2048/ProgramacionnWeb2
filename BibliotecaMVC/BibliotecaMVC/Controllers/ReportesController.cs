using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Reportes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    
    [Authorize(Roles = "Administrador,Bibliotecario")]
    public class ReportesController : Controller
    {
        private const string TipoPdf = "application/pdf";
        private const string TipoExcel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly BibliotecaContext _context;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(BibliotecaContext context, ILogger<ReportesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        public async Task<IActionResult> Index(string? autor, string? categoria)
        {
            _logger.LogInformation("Abriendo la pantalla de reportes de libros");
            var modelo = await ConstruirReporteAsync(autor, categoria);

            
            modelo.OpcionesAutor = await _context.Libros
                .AsNoTracking()
                .Select(l => l.Autor)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            modelo.OpcionesCategoria = await _context.Libros
                .AsNoTracking()
                .Select(l => l.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(modelo);
        }

        
        public async Task<IActionResult> GenerarPdf(string? autor, string? categoria)
        {
            _logger.LogInformation("Generando el reporte PDF de libros");
            try
            {
                var modelo = await ConstruirReporteAsync(autor, categoria);
                byte[] pdf = ReporteLibrosPdf.Generar(modelo);
                return File(pdf, TipoPdf, "ReporteLibros.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al generar el reporte PDF de libros");
                throw;
            }
        }

        
        public async Task<IActionResult> ExportarExcel(string? autor, string? categoria)
        {
            _logger.LogInformation("Generando el reporte Excel de libros");
            try
            {
                var modelo = await ConstruirReporteAsync(autor, categoria);
                byte[] excel = ReporteLibrosExcel.Generar(modelo);
                return File(excel, TipoExcel, "ReporteLibros.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al generar el reporte Excel de libros");
                throw;
            }
        }

       
        private async Task<ReporteLibrosViewModel> ConstruirReporteAsync(string? autor, string? categoria)
        {
            _logger.LogInformation("Consultando libros para el reporte. Autor: {Autor}. Categoría: {Categoria}",
                autor ?? "(todos)", categoria ?? "(todas)");

            
            var consulta = _context.Libros.AsNoTracking();

            
            if (!string.IsNullOrWhiteSpace(autor))
            {
                consulta = consulta.Where(l => l.Autor == autor);
            }

            
            if (!string.IsNullOrWhiteSpace(categoria))
            {
                consulta = consulta.Where(l => l.Categoria == categoria);
            }

            var libros = await consulta
                .OrderBy(l => l.Titulo)
                .ToListAsync();   

            return new ReporteLibrosViewModel
            {
                Libros = libros,
                FiltroAutor = autor,         
                FiltroCategoria = categoria,  
                FechaGeneracion = DateTime.Now,
                GeneradoPor = User.Identity?.Name ?? "Desconocido"
            };
        }
    }
}
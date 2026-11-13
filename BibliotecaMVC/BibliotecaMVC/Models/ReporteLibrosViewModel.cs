namespace BibliotecaMVC.Models
{
    public class ReporteLibrosViewModel
    {
        
        public List<Libro> Libros { get; set; } = new();

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public string GeneradoPor { get; set; } = string.Empty;

        public string? FiltroAutor { get; set; }
        public string? FiltroCategoria { get; set; }

        public List<string> OpcionesAutor { get; set; } = new();
        public List<string> OpcionesCategoria { get; set; } = new();

        public int TotalLibros => Libros.Count;
        public int TotalDisponibles => Libros.Count(l => l.Disponible);
        public int TotalPrestados => Libros.Count(l => !l.Disponible);
        public decimal ValorTotal => Libros.Sum(l => l.Precio);
        public string DescripcionFiltros
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FiltroAutor) && string.IsNullOrWhiteSpace(FiltroCategoria))
                {
                    return "Sin filtros (todos los libros)";
                }

                var autor = string.IsNullOrWhiteSpace(FiltroAutor) ? "Todos" : FiltroAutor;
                var categoria = string.IsNullOrWhiteSpace(FiltroCategoria) ? "Todas" : FiltroCategoria;
                return $"Autor: {autor} | Categoría: {categoria}";
            }
        }
    }
}
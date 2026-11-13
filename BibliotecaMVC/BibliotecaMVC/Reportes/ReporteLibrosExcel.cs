using BibliotecaMVC.Models;
using ClosedXML.Excel;

namespace BibliotecaMVC.Reportes
{
    
    public static class ReporteLibrosExcel
    {
        
        private const string ColorPrincipal = "#1F3864";   
        private const string ColorFilaImpar = "#EEF3FA";   
        private const string ColorBorde = "#B4C6E7";      
        private const string ColorDisponible = "#2E7D32";  
        private const string ColorPrestado = "#C62828";    

        
        private const int TotalColumnas = 6;     
        private const int FilaEncabezados = 5;   

        public static byte[] Generar(ReporteLibrosViewModel modelo)
        {
            
            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Reporte de Libros");

            EscribirTitulo(hoja, modelo);

            
            EscribirEncabezados(hoja);

            
            var ultimaFila = EscribirLibros(hoja, modelo);

            
            hoja.Columns(1, TotalColumnas).AdjustToContents(FilaEncabezados, ultimaFila, 10, 60); 
            hoja.SheetView.FreezeRows(FilaEncabezados);                                   
            hoja.Range(FilaEncabezados, 1, ultimaFila, TotalColumnas).SetAutoFilter();

            CrearHojaResumen(workbook, modelo);
            using var memoria = new MemoryStream();
            workbook.SaveAs(memoria);
            return memoria.ToArray();
        }

        private static void EscribirTitulo(IXLWorksheet hoja, ReporteLibrosViewModel modelo)
        {
            
            hoja.Cell(1, 1).Value = "BibliotecaMVC - Reporte de Libros";
            var titulo = hoja.Range(1, 1, 1, TotalColumnas);
            titulo.Merge();
            titulo.Style.Font.Bold = true;
            titulo.Style.Font.FontSize = 16;
            titulo.Style.Font.FontColor = XLColor.White;
            titulo.Style.Fill.BackgroundColor = XLColor.FromHtml(ColorPrincipal);
            titulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titulo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            hoja.Row(1).Height = 30;

            
            hoja.Cell(2, 1).Value =
                $"Fecha de generación: {modelo.FechaGeneracion:dd/MM/yyyy HH:mm}   |   Generado por: {modelo.GeneradoPor}";
            var fecha = hoja.Range(2, 1, 2, TotalColumnas);
            fecha.Merge();
            fecha.Style.Font.Italic = true;
            fecha.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            
            hoja.Cell(3, 1).Value = $"Filtros: {modelo.DescripcionFiltros}   |   Total de libros: {modelo.TotalLibros}";
            var filtros = hoja.Range(3, 1, 3, TotalColumnas);
            filtros.Merge();
            filtros.Style.Font.Bold = true;
            filtros.Style.Font.FontColor = XLColor.FromHtml(ColorPrincipal);
            filtros.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private static void EscribirEncabezados(IXLWorksheet hoja)
        {
            string[] encabezados = { "ID", "Nombre del libro", "Autor", "Categoría", "Precio", "Disponibilidad" };

            for (var i = 0; i < encabezados.Length; i++)
            {
                hoja.Cell(FilaEncabezados, i + 1).Value = encabezados[i];
            }

            var rango = hoja.Range(FilaEncabezados, 1, FilaEncabezados, TotalColumnas);
            rango.Style.Font.Bold = true;
            rango.Style.Font.FontColor = XLColor.White;
            rango.Style.Fill.BackgroundColor = XLColor.FromHtml(ColorPrincipal);
            rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rango.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            hoja.Row(FilaEncabezados).Height = 22;
        }

       
        private static int EscribirLibros(IXLWorksheet hoja, ReporteLibrosViewModel modelo)
        {
            var fila = FilaEncabezados + 1;

            foreach (var libro in modelo.Libros)
            {
                hoja.Cell(fila, 1).Value = libro.ID;
                hoja.Cell(fila, 2).Value = libro.Titulo;
                hoja.Cell(fila, 3).Value = libro.Autor;
                hoja.Cell(fila, 4).Value = libro.Categoria;
                hoja.Cell(fila, 5).Value = libro.Precio;
                hoja.Cell(fila, 6).Value = libro.Disponible ? "Disponible" : "Prestado";

                
                hoja.Cell(fila, 6).Style.Font.Bold = true;
                hoja.Cell(fila, 6).Style.Font.FontColor =
                    XLColor.FromHtml(libro.Disponible ? ColorDisponible : ColorPrestado);

                
                if ((fila - FilaEncabezados) % 2 == 0)
                {
                    hoja.Range(fila, 1, fila, TotalColumnas).Style.Fill.BackgroundColor = XLColor.FromHtml(ColorFilaImpar);
                }

                fila++;
            }

            var ultimaFila = fila - 1;

            if (modelo.Libros.Count > 0)
            {
                var datos = hoja.Range(FilaEncabezados + 1, 1, ultimaFila, TotalColumnas);

                
                datos.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                datos.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                datos.Style.Border.OutsideBorderColor = XLColor.FromHtml(ColorBorde);
                datos.Style.Border.InsideBorderColor = XLColor.FromHtml(ColorBorde);

               
                hoja.Range(FilaEncabezados + 1, 1, ultimaFila, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; 
                hoja.Range(FilaEncabezados + 1, 5, ultimaFila, 5).Style.NumberFormat.Format = "$#,##0.00";                          
                hoja.Range(FilaEncabezados + 1, 6, ultimaFila, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; 
            }

            return ultimaFila;
        }
        // NUEVO (Actividad 6): hoja con indicadores y libros por categoría
        private static void CrearHojaResumen(XLWorkbook workbook, ReporteLibrosViewModel modelo)
        {
            var hoja = workbook.Worksheets.Add("Resumen");

            // Título
            hoja.Cell(1, 1).Value = "Resumen del reporte de libros";
            var titulo = hoja.Range(1, 1, 1, 2);
            titulo.Merge();
            titulo.Style.Font.Bold = true;
            titulo.Style.Font.FontSize = 14;
            titulo.Style.Font.FontColor = XLColor.White;
            titulo.Style.Fill.BackgroundColor = XLColor.FromHtml(ColorPrincipal);
            titulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Row(1).Height = 24;

            // Indicadores generales: etiqueta en la columna A, valor en la columna B
            hoja.Cell(3, 1).Value = "Filtros aplicados";
            hoja.Cell(3, 2).Value = modelo.DescripcionFiltros;
            hoja.Cell(4, 1).Value = "Total de libros";
            hoja.Cell(4, 2).Value = modelo.TotalLibros;
            hoja.Cell(5, 1).Value = "Disponibles";
            hoja.Cell(5, 2).Value = modelo.TotalDisponibles;
            hoja.Cell(6, 1).Value = "Prestados";
            hoja.Cell(6, 2).Value = modelo.TotalPrestados;
            hoja.Cell(7, 1).Value = "Valor total";
            hoja.Cell(7, 2).Value = modelo.ValorTotal;
            hoja.Cell(7, 2).Style.NumberFormat.Format = "$#,##0.00";

            var indicadores = hoja.Range(3, 1, 7, 2);
            indicadores.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            indicadores.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            indicadores.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            hoja.Range(3, 1, 7, 1).Style.Font.Bold = true;
            hoja.Range(3, 1, 7, 1).Style.Fill.BackgroundColor = XLColor.FromHtml(ColorFilaImpar);

            // Libros por categoría
            hoja.Cell(9, 1).Value = "Categoría";
            hoja.Cell(9, 2).Value = "Cantidad de libros";
            var encabezado = hoja.Range(9, 1, 9, 2);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.FromHtml(ColorPrincipal);
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var fila = 10;
            foreach (var grupo in modelo.Libros.GroupBy(l => l.Categoria).OrderBy(g => g.Key))
            {
                hoja.Cell(fila, 1).Value = grupo.Key;
                hoja.Cell(fila, 2).Value = grupo.Count();
                fila++;
            }

            var tablaCategorias = hoja.Range(9, 1, Math.Max(fila - 1, 9), 2);
            tablaCategorias.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tablaCategorias.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            hoja.Columns(1, 2).AdjustToContents();
        }
    }
}
using System.Globalization;
using BibliotecaMVC.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Reportes
{
    
    public static class ReporteLibrosPdf
    {
        
        private const string ColorPrincipal = "#1F3864";   // azul marino
        private const string ColorAcento = "#2E75B6";      // azul medio
        private const string ColorFilaPar = "#FFFFFF";     // fondo de filas pares
        private const string ColorFilaImpar = "#EEF3FA";   // fondo de filas impares 
        private const string ColorBorde = "#D0D7E2";       // líneas separadoras
        private const string ColorDisponible = "#2E7D32";  // verde
        private const string ColorPrestado = "#C62828";    // rojo
        private static readonly CultureInfo Moneda = CultureInfo.GetCultureInfo("en-US");

        public static byte[] Generar(ReporteLibrosViewModel modelo)
        {
            var documento = Document.Create(contenedor =>
            {
                contenedor.Page(pagina =>
                {
                    pagina.Size(PageSizes.Letter);
                    pagina.Margin(36);  
                    pagina.PageColor(Colors.White);
                    pagina.DefaultTextStyle(texto => texto.FontSize(10).FontColor(Colors.Grey.Darken3));

      



                    pagina.Header().Element(c => ComponerEncabezado(c, modelo));
                    pagina.Content().Element(c => ComponerContenido(c, modelo));
                    pagina.Footer().Element(ComponerPiePagina);
                });
            });

           
            return documento.GeneratePdf();
        }

  
        private static void ComponerEncabezado(IContainer contenedor, ReporteLibrosViewModel modelo)
        {

            contenedor.Column(columna =>
            {
                columna.Item().Row(fila =>
                {
                 
                    fila.RelativeItem().Column(titulos =>
                    {
                        titulos.Item().Text("BIBLIOTECAMVC").FontSize(10).Bold().FontColor(ColorAcento);
                        titulos.Item().Text("Reporte de Libros").FontSize(22).Bold().FontColor(ColorPrincipal);
                        titulos.Item().Text("Sistema de gestión de biblioteca").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    
                    fila.ConstantItem(190).AlignRight().AlignBottom().Column(datos =>
                    {

                        datos.Item().AlignRight().Text($"Fecha de generación: {modelo.FechaGeneracion:dd/MM/yyyy HH:mm}").FontSize(9);
                        datos.Item().AlignRight().Text($"Generado por: {modelo.GeneradoPor}").FontSize(9);
                    });

                });

               
                columna.Item().PaddingTop(6).Text($"Filtros: {modelo.DescripcionFiltros}")
                    .FontSize(9).Italic().FontColor(Colors.Grey.Darken2);


         
                
                columna.Item().PaddingTop(6).LineHorizontal(2).LineColor(ColorPrincipal);
            });
        }

        private static void ComponerContenido(IContainer contenedor, ReporteLibrosViewModel modelo)
        {
            contenedor.PaddingVertical(15).Column(columna =>
            {
                columna.Spacing(15);

                columna.Item().Element(c => ComponerResumen(c, modelo));

                if (modelo.Libros.Count == 0)
                {
                    columna.Item().PaddingTop(20).AlignCenter()
                        .Text("No hay libros para mostrar en este reporte.")
                        .Italic().FontColor(Colors.Grey.Darken1);
                }
                else
                {
                    columna.Item().Element(c => ComponerTabla(c, modelo));
                }
            });
        }


        private static void ComponerResumen(IContainer contenedor, ReporteLibrosViewModel modelo)
        {
            contenedor.Row(fila =>
            {
                fila.Spacing(10);
                fila.RelativeItem().Element(c => Indicador(c, "Total de libros", modelo.TotalLibros.ToString(), ColorPrincipal));
                fila.RelativeItem().Element(c => Indicador(c, "Disponibles", modelo.TotalDisponibles.ToString(), ColorDisponible));
                fila.RelativeItem().Element(c => Indicador(c, "Prestados", modelo.TotalPrestados.ToString(), ColorPrestado));
                fila.RelativeItem().Element(c => Indicador(c, "Valor total", FormatoPrecio(modelo.ValorTotal), ColorAcento));
            });
        }

        private static void Indicador(IContainer contenedor, string titulo, string valor, string colorFondo)
        {
            contenedor.Background(colorFondo).Padding(10).Column(columna =>
            {
                columna.Item().Text(titulo).FontSize(9).FontColor(Colors.White);
                columna.Item().Text(valor).FontSize(16).Bold().FontColor(Colors.White);
            });
        }


        private static void ComponerTabla(IContainer contenedor, ReporteLibrosViewModel modelo)
        {
            contenedor.Table(tabla =>
            {
                
                tabla.ColumnsDefinition(columnas =>
                {
                    columnas.ConstantColumn(35);    
                    columnas.RelativeColumn(3);     
                    columnas.RelativeColumn(2);     
                    columnas.RelativeColumn(1.5f);  
                    columnas.ConstantColumn(65);    
                    columnas.ConstantColumn(70);   
                });

               
                tabla.Header(encabezado =>
                {
                    encabezado.Cell().Element(CeldaEncabezado).Text("ID");
                    encabezado.Cell().Element(CeldaEncabezado).Text("NOMBRE DEL LIBRO");
                    encabezado.Cell().Element(CeldaEncabezado).Text("AUTOR");
                    encabezado.Cell().Element(CeldaEncabezado).Text("CATEGORÍA");
                    encabezado.Cell().Element(CeldaEncabezado).AlignRight().Text("PRECIO");
                    encabezado.Cell().Element(CeldaEncabezado).AlignCenter().Text("ESTADO");
                });

                
                var numeroFila = 0;
                foreach (var libro in modelo.Libros)
                {
                    
                    var fondo = numeroFila % 2 == 0 ? ColorFilaPar : ColorFilaImpar;
                    numeroFila++;

                    tabla.Cell().Element(c => CeldaDato(c, fondo)).Text(libro.ID.ToString());
                    tabla.Cell().Element(c => CeldaDato(c, fondo)).Text(libro.Titulo);
                    tabla.Cell().Element(c => CeldaDato(c, fondo)).Text(libro.Autor);
                    tabla.Cell().Element(c => CeldaDato(c, fondo)).Text(libro.Categoria);
                    tabla.Cell().Element(c => CeldaDato(c, fondo)).AlignRight().Text(FormatoPrecio(libro.Precio));
                    tabla.Cell().Element(c => CeldaDato(c, fondo)).AlignCenter()
                        .Text(libro.Disponible ? "Disponible" : "Prestado")
                        .Bold().FontColor(libro.Disponible ? ColorDisponible : ColorPrestado);
                }
            });
        }

       
        private static void ComponerPiePagina(IContainer contenedor)
        {
            contenedor.Column(columna =>
            {
                columna.Item().LineHorizontal(1).LineColor(ColorBorde);

                columna.Item().PaddingTop(5).Row(fila =>
                {
                    fila.RelativeItem()
                        .Text("BibliotecaMVC - Reporte de Libros")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);

                    fila.RelativeItem().AlignRight()
                        .DefaultTextStyle(estilo => estilo.FontSize(8).FontColor(Colors.Grey.Darken1))
                        .Text(texto =>
                        {
                            texto.Span("Página ");
                            texto.CurrentPageNumber();
                            texto.Span(" de ");
                            texto.TotalPages();
                        });
                });
            });
        }

        
        private static IContainer CeldaEncabezado(IContainer contenedor)
        {
            return contenedor
                .Background(ColorPrincipal)
                .PaddingVertical(7)
                .PaddingHorizontal(5)
                .DefaultTextStyle(texto => texto.FontSize(9).Bold().FontColor(Colors.White));
        }

        
        private static IContainer CeldaDato(IContainer contenedor, string fondo)
        {
            return contenedor
                .Background(fondo)
                .BorderBottom(1)
                .BorderColor(ColorBorde)
                .PaddingVertical(6)
                .PaddingHorizontal(5);
        }

        private static string FormatoPrecio(decimal precio)
        {
            return precio.ToString("C2", Moneda);
        }
    }
}
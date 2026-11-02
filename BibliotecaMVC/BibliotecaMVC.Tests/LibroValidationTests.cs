using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Tests;

public class LibroValidationTests
{
    [Fact]
    // Comprueba que el título del libro sea obligatorio.
    public void TituloEsObligatorio()
    {
        // Crea un libro con título vacío para provocar la validación requerida.
        var libro = LibroValido();
        libro.Titulo = string.Empty;

        // Ejecuta las validaciones definidas mediante atributos en Libro.
        var errores = Validate(libro);

        // Comprueba que exista un error asociado a la propiedad Titulo.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Titulo)));
    }

    [Fact]
    // Comprueba que el título no supere los 100 caracteres.
    public void TituloNoPuedeSuperarLos100Caracteres()
    {
        // Crea un título con 101 caracteres para comprobar el límite máximo.
        var libro = LibroValido();
        libro.Titulo = new string('A', 101);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el error corresponda a la propiedad Titulo.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Titulo)));
    }

    [Fact]
    // Comprueba que el límite exacto de 100 caracteres sea válido.
    public void TituloCon100CaracteresEsValido()
    {
        // Crea un título con exactamente 100 caracteres.
        var libro = LibroValido();
        libro.Titulo = new string('A', 100);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que no haya errores para la propiedad Titulo.
        Assert.DoesNotContain(errores, error => error.MemberNames.Contains(nameof(Libro.Titulo)));
    }

    [Fact]
    // Comprueba que la categoría no supere los 50 caracteres.
    public void CategoriaNoPuedeSuperarLos50Caracteres()
    {
        // Crea un libro válido y una categoría demasiado larga.
        var libro = LibroValido();
        libro.Categoria = new string('B', 51);

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el error corresponda a la propiedad Categoria.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Categoria)));
    }

    [Fact]
    // Comprueba que un libro con datos correctos no tenga errores.
    public void LibroValidoNoTieneErrores()
    {
        // Crea un libro que cumple todas las reglas.
        var libro = LibroValido();

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que la colección de errores esté vacía.
        Assert.Empty(errores);
        Assert.True(libro.Precio >= 0);
        Assert.NotNull(libro.Titulo);
    }

    [Theory]
    [InlineData(-1)]
    // Comprueba varios precios inválidos con el mismo caso de prueba.
    public void PrecioNegativoEsInvalido(decimal precio)
    {
        // Parte de un libro válido y cambia solo el precio.
        var libro = LibroValido();
        libro.Precio = precio;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que el error corresponda al precio.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Libro.Precio)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(99.99)]
    // Comprueba que precios mayores o iguales a 0 sean válidos.
    public void PrecioMayorOIgualACeroEsValido(decimal precio)
    {
        // Parte de un libro válido y asigna el precio del caso.
        var libro = LibroValido();
        libro.Precio = precio;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que no haya error en Precio.
        Assert.DoesNotContain(errores, error => error.MemberNames.Contains(nameof(Libro.Precio)));
        Assert.True(libro.Precio >= 0);
    }

    // Construye un libro que cumple las validaciones, para alterar un solo campo en cada prueba.
    private static Libro LibroValido()
    {
        return new Libro
        {
            Titulo = "Clean Code",
            Autor = "Robert Martin",
            Categoria = "Programación",
            Precio = 35.3M,
            Disponible = true
        };
    }

    // Concentra la ejecución de las validaciones del modelo.
    private static IReadOnlyCollection<ValidationResult> Validate(Libro libro)
    {
        // Crea el contexto de validación para la instancia recibida.
        var context = new ValidationContext(libro);
        // Prepara la colección donde se almacenarán los errores encontrados.
        var errores = new List<ValidationResult>();
        // Ejecuta todas las validaciones, incluso las propiedades no obligatorias.
        Validator.TryValidateObject(libro, context, errores, validateAllProperties: true);
        // Devuelve los errores para que cada prueba pueda analizarlos.
        return errores;
    }
}
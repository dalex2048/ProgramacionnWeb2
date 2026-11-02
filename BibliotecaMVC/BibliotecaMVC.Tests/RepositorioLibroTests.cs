using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Moq;

namespace BibliotecaMVC.Tests;

public class RepositorioLibroTests
{
    [Fact]
    // Configura el mock para devolver libros y comprueba la consulta.
    public void ObtenerTodos_CuandoHayLibros_DevuelveLaListaConfigurada()
    {
        // Prepara datos de prueba controlados.
        var libros = new List<Libro>
        {
            new Libro { ID = 1, Titulo = "A", Autor = "X", Categoria = "C", Precio = 10, Disponible = true },
            new Libro { ID = 2, Titulo = "B", Autor = "Y", Categoria = "C", Precio = 20, Disponible = false }
        };
        // Crea el objeto simulado de la interfaz.
        var mock = new Mock<IRepositorioLibro>();
        // Define qué devolverá ObtenerTodos cuando se invoque.
        mock.Setup(repositorio => repositorio.ObtenerTodos()).Returns(libros);

        // Ejecuta el método simulado.
        var resultado = mock.Object.ObtenerTodos();

        // Comprueba los datos devueltos por el mock.
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.Equal("A", resultado.First().Titulo);
        Assert.True(resultado.First().Disponible);
        Assert.False(resultado.Last().Disponible);
        // Comprueba que la dependencia se haya invocado una vez.
        mock.Verify(repositorio => repositorio.ObtenerTodos(), Times.Once);
    }

    [Fact]
    // Configura el mock para el escenario sin registros.
    public void ObtenerTodos_CuandoNoHayRegistros_DevuelveColeccionVacia()
    {
        // Crea el mock de la interfaz.
        var mock = new Mock<IRepositorioLibro>();
        // Indica que ObtenerTodos devolverá una secuencia vacía.
        mock.Setup(repositorio => repositorio.ObtenerTodos()).Returns(Enumerable.Empty<Libro>());

        // Ejecuta el método simulado.
        var resultado = mock.Object.ObtenerTodos();

        // Comprueba que no haya elementos.
        Assert.Empty(resultado);
        // Comprueba que el método se haya llamado.
        mock.Verify(repositorio => repositorio.ObtenerTodos(), Times.Once);
    }

    [Fact]
    // Prueba la implementación real en memoria, sin base de datos.
    public void RepositorioEnMemoria_ObtenerTodos_DevuelveTresLibros()
    {
        // Crea el repositorio que ya existe en el proyecto.
        var repositorio = new RepositorioEnMemoria();

        // Consulta los libros fijos del repositorio.
        var libros = repositorio.ObtenerTodos().ToList();

        // Comprueba cantidad y un título conocido.
        Assert.NotNull(libros);
        Assert.Equal(3, libros.Count);
        Assert.Equal("Clean Code", libros[0].Titulo);
        Assert.False(libros[1].Disponible);
    }
}
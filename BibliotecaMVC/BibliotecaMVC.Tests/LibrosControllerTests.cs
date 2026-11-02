using BibliotecaMVC.Controllers;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests;

public class LibrosControllerTests
{
    [Fact]
    // Comprueba que Index devuelva todos los Libros guardados.
    public async Task Index_DevuelveLaListaDeLibros()
    {
        // Crea un contexto EF Core con una base de datos en memoria única.
        await using var context = CreateContext();
        // Inserta dos libros de prueba en el contexto.
        context.Libros.AddRange(
            new Libro
            {
                Titulo = "Clean Code",
                Autor = "Robert Martin",
                Categoria = "Programación",
                Precio = 35.3M,
                Disponible = true
            },
            new Libro
            {
                Titulo = "Clean Code tercera edición",
                Autor = "Robert Martin",
                Categoria = "Programación",
                Precio = 40.3M,
                Disponible = true
            });
        // Persiste los libros en la base de datos en memoria.
        await context.SaveChangesAsync();
        // Crea el controlador usando el contexto y un logger simulado.
        var controller = CreateController(context);

        // Ejecuta la acción Index del controlador.
        var result = await controller.Index();

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Obtiene el modelo de la vista como una colección de libros.
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        // Comprueba que la vista contenga los dos autores insertados.
        Assert.Equal(2, libros.Count());
    }
    [Fact]
    // Comprueba el listado cuando no hay libros.
    public async Task Index_SinLibros_DevuelveColeccionVacia()
    {
        // Crea un contexto vacío, sin libros.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Ejecuta Index sobre una base sin registros.
        var result = await controller.Index();
        // Comprueba que se devuelva una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Obtiene el modelo como colección de libros.
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        // Comprueba que la colección no contenga elementos.
        Assert.Empty(libros);
    }
    [Fact]
    // Comprueba que Details devuelva un libro existente.
    public async Task Details_ConLibroExistente_DevuelveElLibro()
    {
        // Crea una base de datos en memoria aislada.
        await using var context = CreateContext();
        // Crea un libro de prueba.
        var libro = new Libro
        {
            Titulo = "C# MVC",
            Autor = "Carlos Martin",
            Categoria = "Programación",
            Precio = 25.9M,
            Disponible = true
        };
        // Añade el libro al contexto.
        context.Libros.Add(libro);
        // Guarda el libro y genera su identificador.
        await context.SaveChangesAsync();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Solicita los detalles usando el identificador generado.
        var result = await controller.Details(libro.ID);
        // Comprueba que la respuesta sea una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que el modelo no sea nulo y que sea el mismo libro guardado.
        Assert.NotNull(view.Model);
        Assert.Same(libro, view.Model);
    }
    [Fact]
    // Comprueba la respuesta cuando el libro no existe.
    public async Task Details_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Solicita los detalles de un identificador inexistente.
        var result = await controller.Details(999);
        // Comprueba que el controlador responda con NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que Create guarde un libro válido y redirija al listado.
    public async Task Create_ConLibroValido_GuardaElLibroYRedirigeAlIndice()
    {
        // Crea una base de datos en memoria vacía.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Construye el libro válido que se enviará a la acción.
        var libro = new Libro
        {
            Titulo = "Clean Architecture",
            Autor = "Robert Martin",
            Categoria = "Programación",
            Precio = 30M,
            Disponible = true
        };
        // Ejecuta la acción Create con el libro.
        var result = await controller.Create(libro);
        // Comprueba que la acción devuelva una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        // Comprueba que se haya guardado un único libro.
        Assert.Equal(1, await context.Libros.CountAsync());
        // Comprueba que el libro guardado tenga el título esperado.
        Assert.Equal("Clean Architecture", (await context.Libros.SingleAsync()).Titulo);
    }

    [Fact]
    // Comprueba que Create no guarde datos cuando el modelo es inválido.
    public async Task Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        // Crea una base de datos en memoria vacía.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Simula un error de validación del título (como haría MVC al recibir el formulario).
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El titulo es obligatorio");
        // Crea el libro que se enviará con el modelo inválido.
        var libro = new Libro
        {
            Autor = "Robert Martin",
            Categoria = "Programación",
            Precio = 10M
        };
        // Ejecuta la acción Create.
        var result = await controller.Create(libro);
        // Comprueba que se devuelva nuevamente la vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que la vista conserve el modelo enviado.
        Assert.Same(libro, view.Model);
        // Comprueba que no se haya guardado ningún libro.
        Assert.Empty(context.Libros);
        // Comprueba que el modelo siga inválido.
        Assert.False(controller.ModelState.IsValid);
    }
    [Fact]
    // Comprueba que Edit actualice los datos de un libro existente.
    public async Task Edit_ConLibroValido_ActualizaElLibro()
    {
        // Genera un nombre único para la base de datos de esta prueba.
        var databaseName = Guid.NewGuid().ToString();
        // Declara la variable donde se almacenará el identificador del libro.
        int libroId;
        // Crea un contexto separado para preparar los datos iniciales.
        await using (var seedContext = CreateContext(databaseName))
        {
            // Crea el libro original.
            var libro = new Libro
            {
                Titulo = "Título original",
                Autor = "Autor original",
                Categoria = "Programación",
                Precio = 10M,
                Disponible = false
            };
            // Añade el libro al contexto inicial.
            seedContext.Libros.Add(libro);
            // Guarda el libro en la base de datos en memoria.
            await seedContext.SaveChangesAsync();
            // Conserva el identificador generado.
            libroId = libro.ID;
        }
        // Crea otro contexto para simular una nueva petición HTTP.
        await using var context = CreateContext(databaseName);
        // Crea el controlador con el contexto de la petición.
        var controller = CreateController(context);
        // Crea la versión modificada del libro.
        var libroEditado = new Libro
        {
            ID = libroId,
            Titulo = "Título actualizado",
            Autor = "Autor original",
            Categoria = "Programación",
            Precio = 20M,
            Disponible = true
        };
        // Ejecuta la acción Edit.
        var result = await controller.Edit(libroEditado, libroId);
        // Comprueba que la acción redirija después de actualizar.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección vaya al listado.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        // Obtiene el libro sin seguimiento para comprobar el valor persistido.
        var libroGuardado = await context.Libros.AsNoTracking().SingleAsync();
        // Comprueba que el título y el precio se hayan actualizado.
        Assert.Equal("Título actualizado", libroGuardado.Titulo);
        Assert.Equal(20M, libroGuardado.Precio);
        Assert.True(libroGuardado.Disponible);
    }
    [Fact]
    // Comprueba que Edit rechace un identificador distinto al del modelo.
    public async Task Edit_ConIdentificadorDiferente_DevuelveBadRequest()
    {
        // Crea un contexto en memoria.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Crea un libro cuyo Id no coincide con el parámetro de la acción.
        var libro = new Libro
        {
            ID = 1,
            Titulo = "Libro",
            Autor = "Autor",
            Categoria = "Programación"
        };
        // Ejecuta Edit con dos identificadores diferentes.
        var result = await controller.Edit(libro, 2);
        // Comprueba que la respuesta sea BadRequest.
        Assert.IsType<BadRequestResult>(result);
    }
    [Fact]
    // Comprueba que Edit devuelva NotFound si el libro no existe.
    public async Task Edit_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Construye un libro con un identificador inexistente.
        var libro = new Libro
        {
            ID = 999,
            Titulo = "Libro",
            Autor = "Autor",
            Categoria = "Programación",
            Precio = 5M
        };
        // Ejecuta Edit con el identificador inexistente.
        var result = await controller.Edit(libro, 999);
        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    // Comprueba que Edit GET devuelva NotFound si el libro no existe.
    public async Task EditGet_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Solicita el formulario de edición de un id inexistente.
        var result = await controller.Edit(999);
        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    // Comprueba que Delete elimine un libro existente y redirija al listado.
    public async Task Delete_ConLibroExistente_EliminaElLibroYRedirigeAlIndice()
    {
        // Crea una base de datos en memoria.
        await using var context = CreateContext();
        // Crea el libro que se eliminará.
        var libro = new Libro
        {
            Titulo = "Libro para eliminar",
            Autor = "Autor",
            Categoria = "Programación",
            Precio = 15M,
            Disponible = true
        };
        // Añade el libro al contexto.
        context.Libros.Add(libro);
        // Persiste el libro y genera su identificador.
        await context.SaveChangesAsync();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Ejecuta Delete con el identificador del libro.
        var result = await controller.Delete(libro.ID);
        // Comprueba que la acción redirija después de eliminar.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        // Comprueba que ya no queden libros en la base de datos.
        Assert.Empty(context.Libros);
    }
    [Fact]
    // Comprueba que Delete devuelva NotFound si el libro no existe.
    public async Task Delete_ConLibroInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Ejecuta Delete con un identificador inexistente.
        var result = await controller.Delete(999);
        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que el logger simulado se invoque al consultar el listado.
    public async Task Index_InvocaElLoggerDeInformacion()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el mock del logger para poder verificar la llamada.
        var logger = new Mock<ILogger<LibrosController>>();
        // Crea el controlador con el logger simulado.
        var controller = new LibrosController(context, logger.Object);
        // Ejecuta Index.
        await controller.Index();
        // Verifica que se haya registrado el mensaje de consulta.
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((estado, _) => estado.ToString()!.Contains("Consultando TODOS los libros")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static LibrosController CreateController(BibliotecaContext context)
    {
        var logger = new Mock<ILogger<LibrosController>>();
        return new LibrosController(context, logger.Object);
    }

    private static BibliotecaContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<BibliotecaContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;
        return new BibliotecaContext(options);
    }
    
}
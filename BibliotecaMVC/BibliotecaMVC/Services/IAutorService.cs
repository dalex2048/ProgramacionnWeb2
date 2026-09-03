using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public interface IAutorService
    {
        IEnumerable<Autor> ObtenerTodos();
        Autor? ObtenerPorId(int id);
    }
}

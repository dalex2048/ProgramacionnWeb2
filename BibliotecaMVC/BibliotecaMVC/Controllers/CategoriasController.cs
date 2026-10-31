using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Consultando el listado de categorías");
            try
            {
                var categorias = new List<Categorias>();
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                categorias.Add(new Categorias
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                });
                            }
                        }
                    }
                }

                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de categorías");
                throw;
            }
            
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la creación de una categoría");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categorias categorias)
        {
            _logger.LogInformation("Creando una categoría");
            if (categorias == null || string.IsNullOrWhiteSpace(categorias.Nombre))
            {
                _logger.LogWarning("La categoría enviada no cumple con los requerimientos mínimos");
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categorias);
            }
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categorias.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categorias.Descripcion ?? System.DBNull.Value);
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Categoria guardada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al Crear una categoria");
                throw;
            }
            
        }

        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando la operación de edición de la categoría con id {Id}", id);
            try
            {
                Categorias categoria = null;
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);
                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                categoria = new Categorias
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                };
                            }
                        }
                    }
                }
                if (categoria == null)
                {
                    _logger.LogWarning("No se encontró la categoría con id {Id}", id);
                    return NotFound();
                }
                if (categoria == null)
                {
                    _logger.LogWarning("No se encontró la categoría con id {Id}", id);
                    return NotFound();
                }
                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al iniciar la edición de la categoría con id {Id}", id);
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categorias categorias)
        {
            _logger.LogInformation("Editando la categoría con id {Id}", categorias.ID);
            if (categorias == null || string.IsNullOrWhiteSpace(categorias.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categorias);
            }
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID = @ID";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", categorias.ID);
                        comando.Parameters.AddWithValue("@Nombre", categorias.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categorias.Descripcion ?? System.DBNull.Value);
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Categoria actualizada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar la categoría con id {Id}", categorias.ID);
                throw;
            }
            
        }
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando la operación de eliminación de la categoría con id {Id}", id);
            try
            {
                Categorias categorias = null;
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);
                        conexion.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                categorias = new Categorias
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                };
                            }
                        }
                    }


                }
                if (categorias == null)
                {
                    _logger.LogWarning("No se encontró la categoría con id {Id}", id);
                    return NotFound();
                }
                return View(categorias);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Se produjo un error al iniciar la eliminación de la categoría con id {Id}", id);
                throw;
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation("Eliminando la categoría con id {Id}", id);
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Categorias WHERE ID = @ID";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Categoria eliminada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar la categoría con id {Id}", id);
                throw;
            }
            
        }
    }
}

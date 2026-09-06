using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using Microsoft.Data.SqlClient; 

namespace BibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        public CategoriasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
        }
        public IActionResult Index()
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

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categorias categorias)
        {
            if (categorias == null || string.IsNullOrWhiteSpace(categorias.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categorias);
            }
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

        public IActionResult Edit(int id)
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
                return NotFound();
            }
            return View(categoria);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categorias categorias)
        {
            if (categorias == null || string.IsNullOrWhiteSpace(categorias.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
                return View(categorias);
            }
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
        public IActionResult Delete(int id)
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
                return NotFound();
            }
            return View(categorias);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var conexion = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Categorias WHERE ID = @ID";
                using ( var comando = new SqlCommand( sql, conexion))
                {
                    comando.Parameters.AddWithValue("@ID", id);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            TempData["SuccessMessage"] = "Categoria eliminada correctamente";
            return RedirectToAction("Index");
        }
    }
}

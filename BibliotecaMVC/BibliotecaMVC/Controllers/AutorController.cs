using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
namespace BibliotecaMVC.Controllers
{
    public class AutorController : Controller
    {
        private static List<Autor> autores = new List<Autor>
        {
                new Autor
                {
                    ID = 1,
                    Nombre = "Dennis",
                    Apellido = "Ritchie",
                    Nacionalidad = "Estadounidense",
                    FechaNacimiento = new DateOnly(1941, 9, 9),
                    Activo = false
                },
                new Autor
                {
                    ID = 2,
                    Nombre = "Linus",
                    Apellido = "Torvalds",
                    Nacionalidad = "Finlandés",
                    FechaNacimiento = new DateOnly(1969, 12, 28),
                    Activo = true
                },
                new Autor
                {
                    ID = 3,
                    Nombre = "Ada",
                    Apellido = "Lovelace",
                    Nacionalidad = "Británica",
                    FechaNacimiento = new DateOnly(1815, 12, 10),
                    Activo = false
                },
                new Autor
                {
                    ID = 4,
                    Nombre = "Anders",
                    Apellido = "Hejlsberg",
                    Nacionalidad = "Danés",
                    FechaNacimiento = new DateOnly(1960, 12, 2),
                    Activo = true
                },
                new Autor
                {
                    ID = 5,
                    Nombre = "Martin",
                    Apellido = "Fowler",
                    Nacionalidad = "Británico",
                    FechaNacimiento = new DateOnly(1963, 12, 18),
                    Activo = true
                }
        };
        public IActionResult Index()
        {
            ViewBag.Autores = autores;
            return View();
        }

        public IActionResult Details(int id)
        {
            var autor = autores.FirstOrDefault(x => x.ID == id);
            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            if (autores.Any())
            {
                autor.ID = autores.Max(x => x.ID) + 1;
            }
            else
            {
                autor.ID = 1;
            }
            autores.Add(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var autor = autores.FirstOrDefault(x =>x.ID == id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }
        [HttpPost,  ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            var autor = autores.FirstOrDefault(x =>x.ID == id);
            if(autor == null)
            {
                return NotFound();
            }
            autores.Remove(autor);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            var autor = autores.FirstOrDefault(y => y.ID == id);
            if (autor == null)
                return NotFound();
            return View(autor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Autor autor)
        {
            if (id != autor.ID)
                return NotFound();
            if (!ModelState.IsValid)
                return View(autor);
            var _autor = autores.FirstOrDefault(x =>x.ID == id);
            if (_autor == null)
                return NotFound();

            _autor.Nombre = autor.Nombre;
            _autor.Apellido = autor.Apellido;
            _autor.Nacionalidad = autor.Nacionalidad;
            _autor.FechaNacimiento = autor.FechaNacimiento;
            _autor.Activo = autor.Activo;
            return RedirectToAction(nameof(Index));
        }

    }
}

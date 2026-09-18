using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;


namespace BibliotecaMVC.Data
{
    public class BibliotecaContext: DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options): base(options)
        {
            
        }
        public DbSet<Autor> Autores {  get; set; }
        public DbSet<Libro> Libros { get; set; }

    }
}

namespace BibliotecaMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    public class Libro
    {
    public int ID { get; set; }
    [Required(ErrorMessage = "El titulo es obligatorio")]
    [StringLength(100)]
    public string Titulo { get; set; }
    [Required(ErrorMessage = "El autor es obligatorio")]
    [StringLength(100)]
    public string Autor {  get; set; }
    [Required(ErrorMessage ="La categoria es obligatiria")]
    [StringLength(50)]
    public string Categoria { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, int.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public decimal Precio { get; set; }
    public bool Disponible { get; set; }



    }

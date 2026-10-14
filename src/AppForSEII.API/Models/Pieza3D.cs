using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class Pieza3D
{
    [Key]
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string Nombre { get; set; }

    [Precision(18, 2)]
    [Range(0.1, 10000.0, ErrorMessage = "El peso debe ser mayor a 0")]
    public decimal Peso { get; set; }

    [Required]
    public CategoriaPieza Categoria { get; set; }

    // Relación M:N con Material
    public ICollection<Material> MaterialesValidos { get; set; }

    // Relación 1:N con LineaEncargo
    public List<LineaEncargo> LineasEncargo { get; set; }

    // 1. Constructor por defecto (obligatorio para Entity Framework Core)
    public Pieza3D()
    {
        Nombre = string.Empty;
        MaterialesValidos = new HashSet<Material>();
        LineasEncargo = new List<LineaEncargo>();
    }

    // 2. Constructor parametrizado para facilitar la creación de instancias
    public Pieza3D(string nombre, decimal peso, CategoriaPieza categoria)
    {
        Nombre = nombre;
        Peso = peso;
        Categoria = categoria;
        MaterialesValidos = new HashSet<Material>();
        LineasEncargo = new List<LineaEncargo>();
    }
}
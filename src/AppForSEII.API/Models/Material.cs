using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class Material
{
    [Key]
    public int Id { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
    public string Nombre { get; set; }

    [Precision(18, 2)]
    public decimal PrecioPorGramo { get; set; }

    [Precision(18, 2)]
    [Range(0.0, 100000.0, ErrorMessage = "El stock debe ser un valor positivo")]
    public decimal StockGramos { get; set; }

    // Relación 1:N con LineaEncargo
    public List<LineaEncargo> LineasEncargo { get; set; }

    // Relación M:N con Pieza3D
    public ICollection<Pieza3D> Piezas3D { get; set; }

    // 1. Constructor por defecto (obligatorio para Entity Framework Core)
    public Material()
    {
        Nombre = string.Empty;
        LineasEncargo = new List<LineaEncargo>();
        Piezas3D = new List<Pieza3D>();
    }

    // 2. Constructor parametrizado para facilitar la creación de instancias
    public Material(string nombre, decimal precioPorGramo, decimal stockGramos)
    {
        Nombre = nombre;
        PrecioPorGramo = precioPorGramo;
        StockGramos = stockGramos;
        LineasEncargo = new List<LineaEncargo>();
        Piezas3D = new List<Pieza3D>();
    }
}
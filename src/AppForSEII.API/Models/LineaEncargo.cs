using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

public class LineaEncargo
{
    [Key]
    public int Id { get; set; }

    [Range(1, 100, ErrorMessage = "La cantidad debe ser entre 1 y 100")]
    public int Cantidad { get; set; }

    [Precision(18, 2)]
    public decimal PrecioUnidad { get; set; }

    [Precision(18, 2)]
    public decimal Subtotal { get; set; }

    // --- CLAVES AJENAS (Foreign Keys) para las relaciones ---
    public int EncargoImpresionId { get; set; }
    public int Pieza3DId { get; set; }
    public int MaterialId { get; set; }

    // --- PROPIEDADES DE NAVEGACIÓN (sin = null!) ---
    public required EncargoImpresion EncargoImpresion { get; set; }
    public required Pieza3D Pieza3D { get; set; }
    public required Material Material { get; set; }

    // 1. Constructor por defecto (obligatorio para Entity Framework Core)
    public LineaEncargo()
    {
    }

    // 2. Constructor parametrizado para facilitar la creación
    public LineaEncargo(int cantidad, decimal precioUnidad, decimal subtotal, int encargoImpresionId, int pieza3DId, int materialId)
    {
        Cantidad = cantidad;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
        EncargoImpresionId = encargoImpresionId;
        Pieza3DId = pieza3DId;
        MaterialId = materialId;
    }
}
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

    // Propiedades de navegación
    public EncargoImpresion EncargoImpresion { get; set; } = null!;
    public Pieza3D Pieza3D { get; set; } = null!;
    public Material Material { get; set; } = null!;
}
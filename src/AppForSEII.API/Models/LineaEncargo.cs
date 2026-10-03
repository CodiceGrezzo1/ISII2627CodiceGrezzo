namespace AppForSEII.API.Models;

public class LineaEncargo
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnidad { get; set; }
    public decimal Subtotal { get; set; }

    // Relación 1:N hacia EncargoImpresion (entidad principal)
    public EncargoImpresion EncargoImpresion { get; set; } = null!;

    public Pieza3D Pieza3D { get; set; } = null!;

    public Material Material { get; set; } = null!;
}
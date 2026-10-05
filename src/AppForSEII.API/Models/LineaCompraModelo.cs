public class LineaCompraModelo
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CantidadLicencias { get; set; }

    [Required]
    public decimal PrecioUnidad { get; set; }

    [Required]
    public decimal Subtotal { get; set; }

    public CompraModelo3D CompraModelo3D { get; set; } = null!;
    public Modelo3D Modelo3D { get; set; } = null!;
}
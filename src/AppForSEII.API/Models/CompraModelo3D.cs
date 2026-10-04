public class CompraModelo3D
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime FechaCompra { get; set; } = DateTime.Now;

    [Required]
    public string NombreCliente { get; set; } = string.Empty;

    [Required]
    public string ApellidosCliente { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required]
    public string DireccionFacturacion { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal PrecioTotal { get; set; }

    [Required]
    public MetodoPago MetodoPago { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public List<LineaCompraModelo> LineasCompraModelo { get; set; } = new List<LineaCompraModelo>();
}
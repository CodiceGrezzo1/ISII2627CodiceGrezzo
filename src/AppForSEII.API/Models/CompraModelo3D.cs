public class CompraModelo3D
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime FechaCompra { get; set; }

    [Required]
    public string NombreCliente { get; set; }

    [Required]
    public string ApellidosCliente { get; set; }

    [Required]
    [EmailAddress]
    public string CorreoElectronico { get; set; }

    [Required]
    public string DireccionFacturacion { get; set; }

    public string? Descripcion { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal PrecioTotal { get; set; }

    [Required]
    public MetodoPago MetodoPago { get; set; }

    public required Cliente Cliente { get; set; }

    public List<LineaCompraModelo> LineasCompraModelo { get; set; }

    public CompraModelo3D()
    {
        FechaCompra = DateTime.Now;
        NombreCliente = string.Empty;
        ApellidosCliente = string.Empty;
        CorreoElectronico = string.Empty;
        DireccionFacturacion = string.Empty;
        LineasCompraModelo = new List<LineaCompraModelo>();
    }

    
    public CompraModelo3D(string nombreCliente, string apellidosCliente, string correoElectronico, string direccionFacturacion, decimal precioTotal, MetodoPago metodoPago, Cliente cliente)
    {
        FechaCompra = DateTime.Now;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        Cliente = cliente;
        LineasCompraModelo = new List<LineaCompraModelo>();
    }
}
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

    public CompraModelo3D CompraModelo3D { get; set; }
    public Modelo3D Modelo3D { get; set; }


    public LineaCompraModelo()
    {
        CompraModelo3D = null!;
        Modelo3D = null!;
    }

    public LineaCompraModelo(int cantidadLicencias, decimal precioUnidad, decimal subtotal, CompraModelo3D compraModelo3D, Modelo3D modelo3D)
    {
        CantidadLicencias = cantidadLicencias;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
        CompraModelo3D = compraModelo3D;
        Modelo3D = modelo3D;
    }
}
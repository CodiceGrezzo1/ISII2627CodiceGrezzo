public class Modelo3D
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Categoria { get; set; }

    [Required]
    public FormatoModelo3D Formato { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }

    public LicenciaModelo3D LicenciaModelo3D { get; set; }

    public List<LineaCompraModelo> LineasCompraModelo { get; set; }

    public Modelo3D()
    {
        Nombre = string.Empty;
        Categoria = string.Empty;
        LicenciaModelo3D = null!;
        LineasCompraModelo = new List<LineaCompraModelo>();
    }

    public Modelo3D(string nombre, string categoria, FormatoModelo3D formato, decimal precio, LicenciaModelo3D licenciaModelo3D)
    {
        Nombre = nombre;
        Categoria = categoria;
        Formato = formato;
        Precio = precio;
        LicenciaModelo3D = licenciaModelo3D;
        LineasCompraModelo = new List<LineaCompraModelo>();
    }
}
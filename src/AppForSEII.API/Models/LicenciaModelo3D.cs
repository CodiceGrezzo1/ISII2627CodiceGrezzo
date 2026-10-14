public class LicenciaModelo3D
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public DateTime FechaExpiracion { get; set; }

    public List<Modelo3D> Modelos3D { get; set; }


    public LicenciaModelo3D()
    {
        Nombre = string.Empty;
        Modelos3D = new List<Modelo3D>();
    }

    public LicenciaModelo3D(string nombre, DateTime fechaExpiracion)
    {
        Nombre = nombre;
        FechaExpiracion = fechaExpiracion;
        Modelos3D = new List<Modelo3D>();
    }
}
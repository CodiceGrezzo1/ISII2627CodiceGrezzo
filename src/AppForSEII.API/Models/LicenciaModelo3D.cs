public class LicenciaModelo3D
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public DateTime FechaExpiracion { get; set; }

    public List<Modelo3D> Modelos3D { get; set; } = new List<Modelo3D>();
}
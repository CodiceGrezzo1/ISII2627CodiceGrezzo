namespace AppForSEII.API.Models;

public class Pieza3D
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public CategoriaPieza Categoria { get; set; }
    
    
    // public ICollection<Material> MaterialesValidos { get; set; } = new HashSet<Material>();
}
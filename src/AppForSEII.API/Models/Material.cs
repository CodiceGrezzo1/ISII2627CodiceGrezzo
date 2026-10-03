namespace AppForSEII.API.Models;

public class Material
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Añade aquí el resto de atributos de tu modelo, por ejemplo:
    public decimal PrecioPorGramo { get; set; }
    
    public decimal StockGramos { get; set; }

    public ICollection<Pieza3D> Piezas3D { get; set; } = new List<Pieza3D>();

    // Relación 1:N con LineaEncargo (rol: material seleccionado)
    public List<LineaEncargo> LineasEncargo { get; set; } = new List<LineaEncargo>();
}
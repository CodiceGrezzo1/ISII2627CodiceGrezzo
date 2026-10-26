using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.Modelo3DDTOs;

public class Modelo3DForListDTO
{
    public Modelo3DForListDTO(int id, string nombre, string categoria, string formato, decimal precio)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria;
        Formato = formato;
        Precio = precio;
    }

    [Range(1, int.MaxValue, ErrorMessage = "El Id debe ser mayor que 0.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 50 caracteres.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "La categoría debe tener entre 1 y 50 caracteres.")]
    public string Categoria { get; set; }

    [Required(ErrorMessage = "El formato es obligatorio.")]
    public string Formato { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Modelo3DForListDTO dto &&
               Id == dto.Id &&
               Nombre == dto.Nombre &&
               Categoria == dto.Categoria &&
               Formato == dto.Formato &&
               Precio == dto.Precio;
    }

    public override int GetHashCode() => HashCode.Combine(Id, Nombre, Categoria, Formato, Precio);
}
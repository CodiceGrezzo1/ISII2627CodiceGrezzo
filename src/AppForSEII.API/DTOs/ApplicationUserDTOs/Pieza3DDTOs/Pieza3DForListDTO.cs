using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.Pieza3DDTOs;

public class Pieza3DForListDTO
{
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string Nombre { get; set; }

    [Range(0.1, 10000.0, ErrorMessage = "El peso debe ser mayor a 0")]
    public decimal Peso { get; set; }

    [StringLength(50, ErrorMessage = "El nombre de la categoría no puede superar los 50 caracteres")]
    public string Categoria { get; set; }

    // Constructor vacío
    public Pieza3DForListDTO()
    {
        Nombre = string.Empty;
        Categoria = string.Empty;
    }

    // Constructor con parámetros para el .Select() del controlador
    public Pieza3DForListDTO(int id, string nombre, decimal peso, string categoria)
    {
        Id = id;
        Nombre = nombre;
        Peso = peso;
        Categoria = categoria;
    }
}
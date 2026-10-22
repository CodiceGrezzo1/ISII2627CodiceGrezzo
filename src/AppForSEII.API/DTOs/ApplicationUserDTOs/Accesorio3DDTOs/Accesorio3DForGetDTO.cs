using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.Accesorio3DDTOs;

public class Accesorio3DForGetDTO
{
    public Accesorio3DForGetDTO(int id, string nombre, decimal precio, CategoriaAccesorio categoria)
    {
        Id = id;
        Nombre = nombre;
        Precio = precio;
        Categoria = categoria.ToString();
    }

    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "Nombre cannot be longer than 50 characters.")]
    public string Nombre { get; set; }

    public decimal Precio { get; set; }

    public string Categoria { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Accesorio3DForGetDTO dto &&
               Id == dto.Id &&
               Nombre == dto.Nombre &&
               Precio == dto.Precio &&
               Categoria == dto.Categoria;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Precio, Categoria);
    }
}
using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ApplicationUserDTOs.Accesorio3DDTOs;

public class Accesorio3DForGetDTO
{
    public Accesorio3DForGetDTO(int id, string nombre, CategoriaAccesorio categoria,
        string compatibilidad, int cantidadDisponible, decimal precio)
    {
        Id = id;
        Nombre = nombre;
        Categoria = categoria.ToString();
        Compatibilidad = compatibilidad;
        CantidadDisponible = cantidadDisponible;
        Precio = precio;
    }

    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "Nombre cannot be longer than 50 characters.")]
    public string Nombre { get; set; }

    public string Categoria { get; set; }

    public string Compatibilidad { get; set; }

    public int CantidadDisponible { get; set; }

    public decimal Precio { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Accesorio3DForGetDTO dto &&
               Id == dto.Id &&
               Nombre == dto.Nombre &&
               Categoria == dto.Categoria &&
               Compatibilidad == dto.Compatibilidad &&
               CantidadDisponible == dto.CantidadDisponible &&
               Precio == dto.Precio;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Categoria, Compatibilidad, CantidadDisponible, Precio);
    }
}  
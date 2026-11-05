
using System;
using AppForSEII.API.DTOs.Pieza3DDTOs;

namespace AppForSEII.API.DTOs.EncargoImpresionDTOs;

public class LineaEncargoForGetDTO
{
    public int Id { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnidad { get; set; }

    public decimal Subtotal { get; set; }

    public Pieza3DForListDTO Pieza3D { get; set; }

    public LineaEncargoForGetDTO()
    {
        Pieza3D = new Pieza3DForListDTO();
    }

    public LineaEncargoForGetDTO(
        int id,
        int cantidad,
        decimal precioUnidad,
        decimal subtotal,
        Pieza3DForListDTO pieza3D)
    {
        Id = id;
        Cantidad = cantidad;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
        Pieza3D = pieza3D;
    }

    public override bool Equals(object? obj)
    {
        return obj is LineaEncargoForGetDTO dto &&
               Id == dto.Id &&
               Cantidad == dto.Cantidad &&
               PrecioUnidad == dto.PrecioUnidad &&
               Subtotal == dto.Subtotal &&
               EqualityComparer<Pieza3DForListDTO>.Default.Equals(Pieza3D, dto.Pieza3D);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Cantidad, PrecioUnidad, Subtotal, Pieza3D);
    }
}

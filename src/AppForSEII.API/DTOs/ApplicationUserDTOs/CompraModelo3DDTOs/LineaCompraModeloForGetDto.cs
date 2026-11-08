using System;
using System.ComponentModel.DataAnnotations;
using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class LineaCompraModeloForGetDto
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int CantidadLicencias { get; set; }

    public decimal PrecioUnidad { get; set; }

    public decimal Subtotal { get; set; }

    public Modelo3DForListDTO Modelo3D { get; set; }

    public LineaCompraModeloForGetDto()
    {
        Modelo3D = new Modelo3DForListDTO();
    }

    public LineaCompraModeloForGetDto(
        int id,
        int cantidadLicencias,
        decimal precioUnidad,
        decimal subtotal,
        Modelo3DForListDTO modelo3D)
    {
        Id = id;
        CantidadLicencias = cantidadLicencias;
        PrecioUnidad = precioUnidad;
        Subtotal = subtotal;
        Modelo3D = modelo3D;
    }

    public override bool Equals(object? obj)
    {
        return obj is LineaCompraModeloForGetDto dto &&
               Id == dto.Id &&
               CantidadLicencias == dto.CantidadLicencias &&
               PrecioUnidad == dto.PrecioUnidad &&
               Subtotal == dto.Subtotal &&
               Equals(Modelo3D, dto.Modelo3D);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, CantidadLicencias, PrecioUnidad, Subtotal, Modelo3D);
    }
}
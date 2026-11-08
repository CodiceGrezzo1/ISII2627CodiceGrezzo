using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class LineaCompraModeloForCreateDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad de licencias debe ser al menos 1")]
    public int CantidadLicencias { get; set; }

    public int Modelo3DId { get; set; }

    public LineaCompraModeloForCreateDTO(int cantidadLicencias, int modelo3DId)
    {
        CantidadLicencias = cantidadLicencias;
        Modelo3DId = modelo3DId;
    }

    public override bool Equals(object? obj)
    {
        return obj is LineaCompraModeloForCreateDTO dto &&
               CantidadLicencias == dto.CantidadLicencias &&
               Modelo3DId == dto.Modelo3DId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CantidadLicencias, Modelo3DId);
    }
}
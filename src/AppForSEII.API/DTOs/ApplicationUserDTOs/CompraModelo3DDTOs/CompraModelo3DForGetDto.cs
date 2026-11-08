using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class CompraModelo3DForGetDto
{
    public int Id { get; set; }

    public DateTime FechaCompra { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreCliente { get; set; }

    [Required]
    [StringLength(100)]
    public string ApellidosCliente { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string CorreoElectronico { get; set; }

    [Required]
    [StringLength(200)]
    public string DireccionFacturacion { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public decimal PrecioTotal { get; set; }

    [Required]
    public string MetodoPago { get; set; }

    public List<LineaCompraModeloForGetDto> LineasCompraModelo { get; set; }

    public CompraModelo3DForGetDto()
    {
        NombreCliente = string.Empty;
        ApellidosCliente = string.Empty;
        CorreoElectronico = string.Empty;
        DireccionFacturacion = string.Empty;
        MetodoPago = string.Empty;
        LineasCompraModelo = new List<LineaCompraModeloForGetDto>();
    }

    public CompraModelo3DForGetDto(
        int id,
        DateTime fechaCompra,
        string nombreCliente,
        string apellidosCliente,
        string correoElectronico,
        string direccionFacturacion,
        string? descripcion,
        decimal precioTotal,
        string metodoPago)
    {
        Id = id;
        FechaCompra = fechaCompra;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        Descripcion = descripcion;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        LineasCompraModelo = new List<LineaCompraModeloForGetDto>();
    }

    public override bool Equals(object? obj)
    {
        return obj is CompraModelo3DForGetDto dto &&
               Id == dto.Id &&
               FechaCompra == dto.FechaCompra &&
               NombreCliente == dto.NombreCliente &&
               ApellidosCliente == dto.ApellidosCliente &&
               CorreoElectronico == dto.CorreoElectronico &&
               DireccionFacturacion == dto.DireccionFacturacion &&
               Descripcion == dto.Descripcion &&
               PrecioTotal == dto.PrecioTotal &&
               MetodoPago == dto.MetodoPago &&
               LineasCompraModelo.SequenceEqual(dto.LineasCompraModelo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            HashCode.Combine(Id, FechaCompra, NombreCliente, ApellidosCliente),
            HashCode.Combine(CorreoElectronico, DireccionFacturacion, Descripcion),
            HashCode.Combine(PrecioTotal, MetodoPago));
    }
}
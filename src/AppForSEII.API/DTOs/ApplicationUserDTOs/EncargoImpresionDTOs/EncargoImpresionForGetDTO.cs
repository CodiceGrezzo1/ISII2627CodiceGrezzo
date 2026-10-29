
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.EncargoImpresionDTOs;

public class EncargoImpresionForGetDTO
{
    public int Id { get; set; }

    public DateTime FechaEncargo { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreCliente { get; set; }

    [Required]
    [StringLength(100)]
    public string ApellidosCliente { get; set; }

    [Required]
    [StringLength(200)]
    public string DireccionEnvio { get; set; }

    [Required]
    [StringLength(15)]
    public string NumeroTelefono { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public decimal PrecioTotal { get; set; }

    [Required]
    public string MetodoPago { get; set; }

    public List<LineaEncargoForGetDTO> LineasEncargo { get; set; }

    public EncargoImpresionForGetDTO()
    {
        NombreCliente = string.Empty;
        ApellidosCliente = string.Empty;
        DireccionEnvio = string.Empty;
        NumeroTelefono = string.Empty;
        MetodoPago = string.Empty;
        LineasEncargo = new List<LineaEncargoForGetDTO>();
    }

    public EncargoImpresionForGetDTO(
        int id,
        DateTime fechaEncargo,
        string nombreCliente,
        string apellidosCliente,
        string direccionEnvio,
        string numeroTelefono,
        string? descripcion,
        decimal precioTotal,
        string metodoPago)
    {
        Id = id;
        FechaEncargo = fechaEncargo;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        Descripcion = descripcion;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        LineasEncargo = new List<LineaEncargoForGetDTO>();
    }

    public override bool Equals(object? obj)
    {
        return obj is EncargoImpresionForGetDTO dto &&
               Id == dto.Id &&
               FechaEncargo == dto.FechaEncargo &&
               NombreCliente == dto.NombreCliente &&
               ApellidosCliente == dto.ApellidosCliente &&
               DireccionEnvio == dto.DireccionEnvio &&
               NumeroTelefono == dto.NumeroTelefono &&
               Descripcion == dto.Descripcion &&
               PrecioTotal == dto.PrecioTotal &&
               MetodoPago == dto.MetodoPago &&
               LineasEncargo.SequenceEqual(dto.LineasEncargo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            HashCode.Combine(Id, FechaEncargo, NombreCliente, ApellidosCliente),
            HashCode.Combine(DireccionEnvio, NumeroTelefono, Descripcion),
            HashCode.Combine(PrecioTotal, MetodoPago));
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.EncargoImpresionDTOs;

public class EncargoImpresionForCreateDTO
{
    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    public string NombreCliente { get; set; }

    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres")]
    public string ApellidosCliente { get; set; }

    [Required(ErrorMessage = "La dirección de envío es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres")]
    public string DireccionEnvio { get; set; }

    [Required(ErrorMessage = "El número de teléfono es obligatorio")]
    [StringLength(15, ErrorMessage = "El teléfono no puede superar los 15 caracteres")]
    public string NumeroTelefono { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio")]
    public MetodoPago MetodoPago { get; set; }

    [Required(ErrorMessage = "El encargo debe contener al menos una línea")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos una línea de encargo")]
    public List<LineaEncargoForCreateDTO> LineasEncargo { get; set; }

    public EncargoImpresionForCreateDTO()
    {
        NombreCliente = string.Empty;
        ApellidosCliente = string.Empty;
        DireccionEnvio = string.Empty;
        NumeroTelefono = string.Empty;
        LineasEncargo = new List<LineaEncargoForCreateDTO>();
    }

    public EncargoImpresionForCreateDTO(
        string nombreCliente,
        string apellidosCliente,
        string direccionEnvio,
        string numeroTelefono,
        string? descripcion,
        MetodoPago metodoPago,
        List<LineaEncargoForCreateDTO> lineasEncargo)
    {
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        Descripcion = descripcion;
        MetodoPago = metodoPago;
        LineasEncargo = lineasEncargo;
    }

    public override bool Equals(object? obj)
    {
        return obj is EncargoImpresionForCreateDTO dto &&
               NombreCliente == dto.NombreCliente &&
               ApellidosCliente == dto.ApellidosCliente &&
               DireccionEnvio == dto.DireccionEnvio &&
               NumeroTelefono == dto.NumeroTelefono &&
               Descripcion == dto.Descripcion &&
               MetodoPago == dto.MetodoPago &&
               LineasEncargo.SequenceEqual(dto.LineasEncargo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NombreCliente, ApellidosCliente, DireccionEnvio, NumeroTelefono, Descripcion, MetodoPago, LineasEncargo);
    }
}
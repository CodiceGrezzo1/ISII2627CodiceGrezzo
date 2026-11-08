using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.CompraModelo3DDTOs;

public class CompraModelo3DForCreateDTO
{
    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    public string NombreCliente { get; set; }

    [Required(ErrorMessage = "Los apellidos del cliente son obligatorios")]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres")]
    public string ApellidosCliente { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
    [StringLength(100, ErrorMessage = "El correo electrónico no puede superar los 100 caracteres")]
    public string CorreoElectronico { get; set; }

    [Required(ErrorMessage = "La dirección de facturación es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres")]
    public string DireccionFacturacion { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio")]
    public MetodoPago MetodoPago { get; set; }

    [Required(ErrorMessage = "La compra debe contener al menos una línea")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos una línea de compra de modelo")]
    public List<LineaCompraModeloForCreateDTO> LineasCompraModelo { get; set; }

    public CompraModelo3DForCreateDTO()
    {
        NombreCliente = string.Empty;
        ApellidosCliente = string.Empty;
        CorreoElectronico = string.Empty;
        DireccionFacturacion = string.Empty;
        LineasCompraModelo = new List<LineaCompraModeloForCreateDTO>();
    }

    public CompraModelo3DForCreateDTO(
        string nombreCliente,
        string apellidosCliente,
        string correoElectronico,
        string direccionFacturacion,
        string? descripcion,
        MetodoPago metodoPago,
        List<LineaCompraModeloForCreateDTO> lineasCompraModelo)
    {
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        CorreoElectronico = correoElectronico;
        DireccionFacturacion = direccionFacturacion;
        Descripcion = descripcion;
        MetodoPago = metodoPago;
        LineasCompraModelo = lineasCompraModelo;
    }

    public override bool Equals(object? obj)
    {
        return obj is CompraModelo3DForCreateDTO dto &&
               NombreCliente == dto.NombreCliente &&
               ApellidosCliente == dto.ApellidosCliente &&
               CorreoElectronico == dto.CorreoElectronico &&
               DireccionFacturacion == dto.DireccionFacturacion &&
               Descripcion == dto.Descripcion &&
               MetodoPago == dto.MetodoPago &&
               LineasCompraModelo.SequenceEqual(dto.LineasCompraModelo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NombreCliente, ApellidosCliente, CorreoElectronico, DireccionFacturacion, Descripcion, MetodoPago, LineasCompraModelo);
    }
}
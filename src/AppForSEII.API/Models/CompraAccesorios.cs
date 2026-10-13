using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System;

namespace AppForSEII.API.Models;

public class CompraAccesorios
{
    [Key]
    public int Id { get; set; }

    public DateTime FechaCompra { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre del cliente debe tener entre 2 y 50 caracteres")]
    public string NombreCliente { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "Los apellidos del cliente deben tener entre 2 y 100 caracteres")]
    public string ApellidosCliente { get; set; }

    [StringLength(200, MinimumLength = 5, ErrorMessage = "La dirección de envío debe tener entre 5 y 200 caracteres")]
    public string DireccionEnvio { get; set; }

    [Phone]
    [StringLength(15, ErrorMessage = "El número de teléfono no puede superar los 15 caracteres")]
    public string NumeroTelefono { get; set; }

    [Precision(18, 2)]
    [Range(0.01, 10000.0, ErrorMessage = "El precio total debe ser mayor a 0")]
    public decimal PrecioTotal { get; set; }

    public MetodoPago MetodoPago { get; set; }
    
    public Cliente Cliente { get; set; }
    
    public List<LineaCompraAccesorio> LineasCompraAccesorio { get; set; }

    // 1. Constructor por defecto (obligatorio para Entity Framework Core)
    public CompraAccesorios()
    {
        FechaCompra = DateTime.Now;
        NombreCliente = string.Empty;
        ApellidosCliente = string.Empty;
        DireccionEnvio = string.Empty;
        NumeroTelefono = string.Empty;
        LineasCompraAccesorio = new List<LineaCompraAccesorio>();
    }

    // 2. Constructor parametrizado para facilitar la creación de instancias
    public CompraAccesorios(string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, decimal precioTotal, MetodoPago metodoPago, Cliente cliente)
    {
        FechaCompra = DateTime.Now;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        PrecioTotal = precioTotal;
        MetodoPago = metodoPago;
        Cliente = cliente;
        LineasCompraAccesorio = new List<LineaCompraAccesorio>();
    }
}
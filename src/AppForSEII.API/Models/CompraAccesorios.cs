using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace AppForSEII.API.Models;

public class CompraAccesorios
{
    [Key]
    public int Id { get; set; }

    public DateTime FechaCompra { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre del cliente debe tener entre 2 y 50 caracteres")]
    public string NombreCliente { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 2, ErrorMessage = "Los apellidos del cliente deben tener entre 2 y 100 caracteres")]
    public string ApellidosCliente { get; set; } = string.Empty;

    [StringLength(200, MinimumLength = 5, ErrorMessage = "La dirección de envío debe tener entre 5 y 200 caracteres")]
    public string DireccionEnvio { get; set; } = string.Empty;

    [Phone]
    [StringLength(15, ErrorMessage = "El número de teléfono no puede superar los 15 caracteres")]
    public string NumeroTelefono { get; set; } = string.Empty;

    [Precision(18, 2)]
    [Range(0.01, 10000.0, ErrorMessage = "El precio total debe ser mayor a 0")]
    public decimal PrecioTotal { get; set; }

    public MetodoPago MetodoPago { get; set; }
public List<LineaCompraAccesorio> LineasCompraAccesorio { get; set; } = new List<LineaCompraAccesorio>();
public Cliente Cliente { get; set; } = null!;

}
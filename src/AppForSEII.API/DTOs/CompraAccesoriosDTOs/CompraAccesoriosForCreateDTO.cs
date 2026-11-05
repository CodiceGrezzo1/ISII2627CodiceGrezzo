using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.CompraAccesoriosDTOs;

public class CompraAccesoriosForCreateDTO
{
    public CompraAccesoriosForCreateDTO() { }

    public CompraAccesoriosForCreateDTO(string userNameCliente, string nombreCliente, string apellidosCliente,
        string direccionEnvio, string numeroTelefono, MetodoPago metodoPago,
        IList<LineaCompraAccesorioDTO> lineas)
    {
        UserNameCliente = userNameCliente;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionEnvio = direccionEnvio;
        NumeroTelefono = numeroTelefono;
        MetodoPago = metodoPago;
        Lineas = lineas;
    }

    [EmailAddress]
    [Required]
    public string UserNameCliente { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Indica tu nombre")]
    public string NombreCliente { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Indica tus apellidos")]
    public string ApellidosCliente { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Indica la dirección de envío")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "La dirección debe tener al menos 10 caracteres")]
    public string DireccionEnvio { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string NumeroTelefono { get; set; } = string.Empty;

    [Required]
    public MetodoPago MetodoPago { get; set; } = MetodoPago.tarjetaCredito;

    public IList<LineaCompraAccesorioDTO> Lineas { get; set; } = new List<LineaCompraAccesorioDTO>();

    public decimal PrecioTotal => Lineas.Sum(l => l.Subtotal);

    public override bool Equals(object? obj)
    {
        return obj is CompraAccesoriosForCreateDTO dto &&
               UserNameCliente == dto.UserNameCliente &&
               NombreCliente == dto.NombreCliente &&
               ApellidosCliente == dto.ApellidosCliente &&
               DireccionEnvio == dto.DireccionEnvio &&
               NumeroTelefono == dto.NumeroTelefono &&
               MetodoPago == dto.MetodoPago &&
               Lineas.SequenceEqual(dto.Lineas) &&
               PrecioTotal == dto.PrecioTotal;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserNameCliente, NombreCliente, ApellidosCliente,
            DireccionEnvio, NumeroTelefono, MetodoPago, PrecioTotal);
    }
}
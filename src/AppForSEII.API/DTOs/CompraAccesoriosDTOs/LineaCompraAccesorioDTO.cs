using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.CompraAccesoriosDTOs;

public class LineaCompraAccesorioDTO
{
    public LineaCompraAccesorioDTO() { }

    public LineaCompraAccesorioDTO(int accesorioId, string nombreAccesorio, int cantidad, decimal precioUnitario)
    {
        AccesorioId = accesorioId;
        NombreAccesorio = nombreAccesorio;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public int AccesorioId { get; set; }

    public string NombreAccesorio { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad mínima es 1")]
    public int Cantidad { get; set; } = 1;

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;

    public override bool Equals(object? obj)
    {
        return obj is LineaCompraAccesorioDTO dto &&
               AccesorioId == dto.AccesorioId &&
               NombreAccesorio == dto.NombreAccesorio &&
               Cantidad == dto.Cantidad &&
               PrecioUnitario == dto.PrecioUnitario;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AccesorioId, NombreAccesorio, Cantidad, PrecioUnitario);
    }
}
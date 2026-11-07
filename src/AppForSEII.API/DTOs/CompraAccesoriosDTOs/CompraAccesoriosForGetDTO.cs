using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.CompraAccesoriosDTOs;

public class CompraAccesoriosForGetDTO : CompraAccesoriosForCreateDTO
{
    public CompraAccesoriosForGetDTO(int id, DateTime fechaCompra, string userNameCliente, string nombreCliente,
        string apellidosCliente, string direccionEnvio, string numeroTelefono, MetodoPago metodoPago,
        IList<LineaCompraAccesorioDTO> lineas)
        : base(userNameCliente, nombreCliente, apellidosCliente, direccionEnvio, numeroTelefono, metodoPago, lineas)
    {
        Id = id;
        FechaCompra = fechaCompra;
    }

    public int Id { get; set; }

    public DateTime FechaCompra { get; set; }

    // FechaCompra no se compara a propósito: en los tests se genera con DateTime.Now
    // y nunca coincide exactamente.
    public override bool Equals(object? obj)
    {
        return obj is CompraAccesoriosForGetDTO dto &&
               base.Equals(obj) &&
               Id == dto.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Id);
    }
}
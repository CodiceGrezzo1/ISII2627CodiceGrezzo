using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ReservaDTOs
{
    public class ReservaDetailDTO
    {
        public int Id { get; set; }
        public DateTime FechaReserva { get; set; }
        public string NombreCliente { get; set; }
        public string? ApellidosCliente { get; set; }
        public string DireccionFacturacion { get; set; }
        public decimal PrecioTotal { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public IList<LineaReservaDTO> Lineas { get; set; }

        public ReservaDetailDTO(int id, DateTime fechaReserva, string nombreCliente, string? apellidosCliente,
            string direccionFacturacion, decimal precioTotal, MetodoPago metodoPago, IList<LineaReservaDTO> lineas)
        {
            Id = id;
            FechaReserva = fechaReserva;
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionFacturacion = direccionFacturacion;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            Lineas = lineas;
        }
    }
}
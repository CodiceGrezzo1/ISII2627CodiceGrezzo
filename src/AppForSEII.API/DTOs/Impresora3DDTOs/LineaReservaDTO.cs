namespace AppForSEII.API.DTOs.ReservaDTOs
{
    // Línea de una reserva tal y como se muestra al cliente en el paso 7 del caso de uso:
    // Nombre, Modelo, Tipo, Descripción, tiempo de reserva, Precio kWh, Precio de reserva y Precio subtotal
    public class LineaReservaDTO
    {
        public int ImpresoraId { get; set; }
        public string ImpresoraNombre { get; set; }
        public string ImpresoraModelo { get; set; }
        public TipoImpresora ImpresoraTipo { get; set; }
        public string? ImpresoraDescripcion { get; set; }
        public decimal PrecioKilovatioHora { get; set; }
        public decimal PrecioReserva { get; set; }
        public TiempoReserva TiempoReserva { get; set; }
        public decimal PrecioSubtotal { get; set; }

        public LineaReservaDTO(int impresoraId, string impresoraNombre, string impresoraModelo,
            TipoImpresora impresoraTipo, string? impresoraDescripcion, decimal precioKilovatioHora,
            decimal precioReserva, TiempoReserva tiempoReserva, decimal precioSubtotal)
        {
            ImpresoraId = impresoraId;
            ImpresoraNombre = impresoraNombre;
            ImpresoraModelo = impresoraModelo;
            ImpresoraTipo = impresoraTipo;
            ImpresoraDescripcion = impresoraDescripcion;
            PrecioKilovatioHora = precioKilovatioHora;
            PrecioReserva = precioReserva;
            TiempoReserva = tiempoReserva;
            PrecioSubtotal = precioSubtotal;
        }
    }
}
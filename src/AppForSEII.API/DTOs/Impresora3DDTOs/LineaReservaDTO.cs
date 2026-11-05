namespace AppForSEII.API.DTOs.ReservaDTOs
{
    public class LineaReservaDTO
    {
        public int ImpresoraId { get; set; }
        public string ImpresoraNombre { get; set; }
        public string ImpresoraModelo { get; set; }
        public TiempoReserva TiempoReserva { get; set; }
        public decimal PrecioSubtotal { get; set; }

        public LineaReservaDTO(int impresoraId, string impresoraNombre, string impresoraModelo, TiempoReserva tiempoReserva, decimal precioSubtotal)
        {
            ImpresoraId = impresoraId;
            ImpresoraNombre = impresoraNombre;
            ImpresoraModelo = impresoraModelo;
            TiempoReserva = tiempoReserva;
            PrecioSubtotal = precioSubtotal;
        }
    }
}
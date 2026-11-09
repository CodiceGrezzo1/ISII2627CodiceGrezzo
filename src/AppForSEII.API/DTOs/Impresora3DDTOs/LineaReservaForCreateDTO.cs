namespace AppForSEII.API.DTOs.ReservaDTOs
{
    // Impresora seleccionada por el cliente y tiempo durante el que la quiere reservar
    public class LineaReservaForCreateDTO
    {
        public LineaReservaForCreateDTO()
        {
        }

        public LineaReservaForCreateDTO(int impresoraId, TiempoReserva tiempoReserva = TiempoReserva.UnaHora)
        {
            ImpresoraId = impresoraId;
            TiempoReserva = tiempoReserva;
        }

        public int ImpresoraId { get; set; }

        // Por defecto las impresoras se reservan una hora (paso 3 del caso de uso)
        public TiempoReserva TiempoReserva { get; set; } = TiempoReserva.UnaHora;
    }
}
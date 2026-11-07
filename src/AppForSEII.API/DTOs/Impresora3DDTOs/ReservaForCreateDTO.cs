namespace AppForSEII.API.DTOs.ReservaDTOs
{
    // Datos que recibe CreateReserva (pasos 5 y 6 del caso de uso)
    public class ReservaForCreateDTO
    {
        public ReservaForCreateDTO()
        {
        }

        public ReservaForCreateDTO(string clienteUserName, string nombreCliente, string apellidosCliente,
            string direccionFacturacion, MetodoPago metodoPago, IList<LineaReservaForCreateDTO> lineas)
        {
            ClienteUserName = clienteUserName ?? throw new ArgumentNullException(nameof(clienteUserName));
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidosCliente = apellidosCliente ?? throw new ArgumentNullException(nameof(apellidosCliente));
            DireccionFacturacion = direccionFacturacion ?? throw new ArgumentNullException(nameof(direccionFacturacion));
            MetodoPago = metodoPago;
            Lineas = lineas ?? throw new ArgumentNullException(nameof(lineas));
        }

        // UserName (correo) del cliente que ha iniciado sesión
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica el usuario del cliente")]
        public string ClienteUserName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica tu nombre")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica tus apellidos")]
        [StringLength(150, ErrorMessage = "Los apellidos no pueden superar los 150 caracteres")]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, indica tu dirección de facturación")]
        [StringLength(200, ErrorMessage = "La dirección de facturación no puede superar los 200 caracteres")]
        public string DireccionFacturacion { get; set; } = string.Empty;

        [Required]
        public MetodoPago MetodoPago { get; set; }

        public IList<LineaReservaForCreateDTO> Lineas { get; set; } = new List<LineaReservaForCreateDTO>();
    }
}
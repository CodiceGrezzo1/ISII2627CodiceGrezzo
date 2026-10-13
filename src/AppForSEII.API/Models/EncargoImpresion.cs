using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models
{
    public class EncargoImpresion
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaEncargo { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string NombreCliente { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public string ApellidosCliente { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "La dirección de envío no puede superar los 200 caracteres.")]
        public string DireccionEnvio { get; set; }

        [Required]
        [Phone]
        [StringLength(15, ErrorMessage = "El teléfono no puede superar los 15 caracteres.")]
        public string NumeroTelefono { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal PrecioTotal { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        // Propiedad de navegación (sin = null!)
        public required Cliente Cliente { get; set; }

        // Relación 1:N con LineaEncargo
        public List<LineaEncargo> LineasEncargo { get; set; }

        // 1. Constructor por defecto (obligatorio para Entity Framework Core)
        public EncargoImpresion()
        {
            FechaEncargo = DateTime.Now;
            NombreCliente = string.Empty;
            ApellidosCliente = string.Empty;
            DireccionEnvio = string.Empty;
            NumeroTelefono = string.Empty;
            LineasEncargo = new List<LineaEncargo>();
        }

        // 2. Constructor parametrizado para facilitar la creación de instancias
        public EncargoImpresion(string nombreCliente, string apellidosCliente, string direccionEnvio, string numeroTelefono, decimal precioTotal, MetodoPago metodoPago, Cliente cliente)
        {
            FechaEncargo = DateTime.Now;
            NombreCliente = nombreCliente;
            ApellidosCliente = apellidosCliente;
            DireccionEnvio = direccionEnvio;
            NumeroTelefono = numeroTelefono;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            Cliente = cliente;
            LineasEncargo = new List<LineaEncargo>();
        }
    }
}
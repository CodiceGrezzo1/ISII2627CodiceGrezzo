namespace AppForSEII.API.Models
{
   public class EncargoImpresion
    {
        [Key]
        public int Id { get; set; }

      
        public DateTime FechaEncargo { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(200, ErrorMessage = "La dirección de envío no puede superar los 200 caracteres.")]
        public string DireccionEnvio { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15, ErrorMessage = "El teléfono no puede superar los 15 caracteres.")]
        public string NumeroTelefono { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal PrecioTotal { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        // public LineaEncargo LineaEncargo { get; set; }
    }
}
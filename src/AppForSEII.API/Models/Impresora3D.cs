using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class Impresora3D
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(100)]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public TipoImpresora Tipo { get; set; } 

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioKilovatioHora { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioReserva { get; set; }






        // Relación: Una impresora puede estar en múltiples líneas de reserva
        public IList<LineaReserva> Lineas { get; set; } = new List<LineaReserva>();
    }
}
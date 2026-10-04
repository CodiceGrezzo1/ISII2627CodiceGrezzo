using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class LineaReserva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TiempoReserva TiempoReserva { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioSubtotal { get; set; }
    }
}
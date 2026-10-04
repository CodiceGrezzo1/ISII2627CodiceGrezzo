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






        
        
        // Relación: Una línea pertenece a una reserva
        public int ReservaImpresoraId { get; set; }
        public ReservaImpresora ReservaImpresora { get; set; } = null!; //para que se quite la advertencia de que puede ser null

        // Relación: Una línea apunta a una impresora concreta
        public int Impresora3DId { get; set; }
        public Impresora3D Impresora3D { get; set; } = null!;
    }
    
}


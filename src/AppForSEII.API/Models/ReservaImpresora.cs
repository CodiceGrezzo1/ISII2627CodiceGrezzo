using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class ReservaImpresora
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime FechaReserva { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DireccionFacturacion { get; set; } = string.Empty;

        [Required]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioTotal { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }





        

        // Relación: Una reserva contiene muchas líneas (Composición)
        public IList<LineaReserva> Lineas { get; set; } = []; //sugerencia de que en esta version de C# no hay que escribir la ruta completa de una lista vacía

        // Relación: Una reserva es realizada por un cliente
        // (El Id es string porque hereda de IdentityUser)
        public string ClienteId { get; set; } = string.Empty;
        public Cliente Cliente { get; set; } = null!;
    }
}
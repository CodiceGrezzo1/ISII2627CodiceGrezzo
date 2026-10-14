using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AppForSEII.API.Models
{
    public class ReservaImpresora
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string ApellidosCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DireccionFacturacion { get; set; } = string.Empty;

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioTotal { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        // Relación: Una reserva contiene muchas líneas (Composición)
        public IList<LineaReserva> Lineas { get; set; } = [];

        // Relación: Una reserva es realizada por un cliente
        // (El Id es string porque hereda de IdentityUser)
        public string ClienteId { get; set; } = string.Empty;
        public required Cliente Cliente { get; set; }

        // 1. Constructor por defecto (obligatorio para Entity Framework Core)
        public ReservaImpresora()
        {
            ApellidosCliente = string.Empty;
            DireccionFacturacion = string.Empty;
            ClienteId = string.Empty;
            Lineas = [];
        }

        // 2. Constructor parametrizado para facilitar la creación de instancias
        public ReservaImpresora(string apellidosCliente, string direccionFacturacion, decimal precioTotal, MetodoPago metodoPago, Cliente cliente)
        {
            ApellidosCliente = apellidosCliente;
            DireccionFacturacion = direccionFacturacion;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            Cliente = cliente;
            ClienteId = cliente != null ? cliente.Id : string.Empty;
            Lineas = [];
        }
    }
}
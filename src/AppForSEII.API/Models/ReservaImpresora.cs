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
        public string? ApellidosCliente { get; set; } // El ? indica que es opcional

        [Required]
        [StringLength(200)]
        public required string DireccionFacturacion { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioTotal { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        // Relación: Una reserva contiene muchas líneas (Composición)
        public IList<LineaReserva> Lineas { get; set; } = [];

        // Relación: Una reserva es realizada por un cliente
        public required string ClienteId { get; set; }
        public required Cliente Cliente { get; set; }

        // 1. Constructor por defecto (obligatorio para Entity Framework Core)
        public ReservaImpresora()
        {
            Lineas = [];
        }

        // 2. Constructor parametrizado para facilitar la creación de instancias
        public ReservaImpresora(string? apellidosCliente, string direccionFacturacion, decimal precioTotal, MetodoPago metodoPago, Cliente cliente)
        {
            ApellidosCliente = apellidosCliente;
            DireccionFacturacion = direccionFacturacion;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            Cliente = cliente;
            ClienteId = cliente?.Id ?? ""; // Obtiene el Id del cliente automáticamente
            Lineas = [];
        }
    }
}
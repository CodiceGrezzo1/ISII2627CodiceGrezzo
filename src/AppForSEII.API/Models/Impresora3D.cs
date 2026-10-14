using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class Impresora3D
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public required string Modelo { get; set; }

        [Required]
        public TipoImpresora Tipo { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; } // El ? indica que es opcional

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioKilovatioHora { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(8, 2)]
        public decimal PrecioReserva { get; set; }

        // 1. Constructor por defecto (obligatorio para Entity Framework Core)
        public Impresora3D()
        {
            // Ya no hace falta ensuciarlo con string.Empty
        }

        // 2. Constructor parametrizado para facilitar la creación de instancias
        public Impresora3D(string nombre, string modelo, TipoImpresora tipo, string? descripcion, decimal precioKilovatioHora, decimal precioReserva)
        {
            Nombre = nombre;
            Modelo = modelo;
            Tipo = tipo;
            Descripcion = descripcion;
            PrecioKilovatioHora = precioKilovatioHora;
            PrecioReserva = precioReserva;
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class Accesorio
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del accesorio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public CategoriaAccesorio Categoria { get; set; }

        [Required(ErrorMessage = "La compatibilidad es obligatoria.")]
        [StringLength(200, ErrorMessage = "La compatibilidad no puede superar los 200 caracteres.")]
        public string Compatibilidad { get; set; }

        [Required(ErrorMessage = "La cantidad disponible es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad disponible no puede ser negativa.")]
        public int CantidadDisponible { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe ser mayor que 0.")]
        public decimal Precio { get; set; }

        public IList<LineaCompraAccesorio> LineaCompraAccesorios { get; set; } = new List<LineaCompraAccesorio>();
    }
}
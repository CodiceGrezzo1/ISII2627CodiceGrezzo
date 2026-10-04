public class LicenciaModelo3D
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public DateTime FechaExpiracion { get; set; }
    }
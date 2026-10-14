namespace AppForSEII.API.Models
{
    public class LineaCompraAccesorio
    {
        public int Id { get; set; }
        
        public string NombreAccesorio { get; set; }
        
        public int Cantidad { get; set; }
        
        public decimal PrecioUnitario { get; set; }

        // Propiedad calculada (no requiere inicialización en el constructor)
        public decimal Subtotal => Cantidad * PrecioUnitario;

        public required Accesorio Accesorio { get; set; }
        
        public required CompraAccesorios CompraAccesorios { get; set; }

        // 1. Constructor por defecto (obligatorio para Entity Framework Core)
        public LineaCompraAccesorio()
        {
            NombreAccesorio = string.Empty;
        }

        // 2. Constructor parametrizado para facilitar la creación de instancias
        public LineaCompraAccesorio(string nombreAccesorio, int cantidad, decimal precioUnitario, Accesorio accesorio, CompraAccesorios compraAccesorios)
        {
            NombreAccesorio = nombreAccesorio;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            Accesorio = accesorio;
            CompraAccesorios = compraAccesorios;
        }

        public override string ToString()
        {
            return $"{NombreAccesorio} x{Cantidad} Subtotal: {Subtotal:C}";
        }
    }
}
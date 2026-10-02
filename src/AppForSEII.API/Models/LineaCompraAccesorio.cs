public class LineaCompraAccesorio
    {
        public int Id { get; set; }
        public string NombreAccesorio { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal => Cantidad * PrecioUnitario;

        public LineaCompraAccesorio(int id, string nombreAccesorio, int cantidad, decimal precioUnitario)
        {
            Id = id;
            NombreAccesorio = nombreAccesorio;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }

        public override string ToString()
        {
            return $"{NombreAccesorio} x{Cantidad} - Subtotal: {Subtotal:C}";
        }
    }

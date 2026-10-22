namespace AppForSEII.API.DTOs.Impresora3DDTOs;

public class ImpresoraParaReservarDTO
{
    public ImpresoraParaReservarDTO()
    {
    }

    public ImpresoraParaReservarDTO(int id, string nombre, string modelo, TipoImpresora tipo,
        string? descripcion, decimal precioKilovatioHora, decimal precioReserva)
    {
        Id = id;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
    }

    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    public TipoImpresora Tipo { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public decimal PrecioKilovatioHora { get; set; }

    public decimal PrecioReserva { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ImpresoraParaReservarDTO dto &&
               Id == dto.Id &&
               Nombre == dto.Nombre &&
               Modelo == dto.Modelo &&
               Tipo == dto.Tipo &&
               Descripcion == dto.Descripcion &&
               PrecioKilovatioHora == dto.PrecioKilovatioHora &&
               PrecioReserva == dto.PrecioReserva;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Modelo, Tipo, Descripcion, PrecioKilovatioHora, PrecioReserva);
    }
}
namespace AppForSEII.API.Models;

public class Cliente: ApplicationUser
{
    public Cliente()
    {
    }
    public Cliente(string id, string name, string surname, string userName, string direccionFacturacion) : base(id, name, surname, userName)
    {
        DireccionFacturacion = direccionFacturacion;
    }

    public string? DireccionFacturacion {get;set;}
    public List<EncargoImpresion> Encargos { get; set; } = new List<EncargoImpresion>();
    List<CompraAccesorios> CompraAccesorios { get; set; } = new List<CompraAccesorios>();

}
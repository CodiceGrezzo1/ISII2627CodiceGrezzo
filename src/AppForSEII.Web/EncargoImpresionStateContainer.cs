// DTOs y clases generadas por NSwag a partir de la API
using AppForSEII.Web.API;

// Namespace de la carpeta donde está guardado este archivo dentro de AppForSEII.Web
namespace AppForSEII.Web;

// Contenedor de estado: guarda en memoria el encargo en preparación y el total
// mientras el usuario navega entre las pantallas del caso de uso
public class EncargoImpresionStateContainer
{
    // Instancia del DTO que se enviará por POST al crear el encargo.
    // Se crea vacía con su lista LineasEncargo inicializada para evitar nulos.
    public EncargoImpresionForCreateDTO Encargo { get; private set; } = new()
    {
        LineasEncargo = new List<LineaEncargoForCreateDTO>()
    };

    // Importe total acumulado de las piezas añadidas al encargo
    public decimal PrecioTotal { get; set; }

    // Evento por si algún componente quiere suscribirse a cambios del estado
    public event Action? OnChange;

    // Método auxiliar para invocar OnChange si tiene suscriptores
    private void NotifyStateChanged() => OnChange?.Invoke();

    // Añade una pieza con un material y cantidad si no estaba ya incluida con ese mismo material,
    // y actualiza el precio total acumulado
    public void AddPiezaToEncargo(Pieza3DForListDTO pieza, int materialId, decimal precioUnidad, int cantidad = 1)
    {
        if (!Encargo.LineasEncargo.Any(l => l.Pieza3DID == pieza.Id && l.MaterialId == materialId))
        {
            Encargo.LineasEncargo.Add(new LineaEncargoForCreateDTO
            {
                Cantidad = cantidad,
                Pieza3DID = pieza.Id,
                MaterialId = materialId
            });

            PrecioTotal += precioUnidad * cantidad;
        }
    }

    // Elimina una línea concreta de la selección y resta su importe del total
    public void RemoveLineaEncargo(LineaEncargoForCreateDTO linea, decimal precioUnidad)
    {
        if (Encargo.LineasEncargo.Remove(linea))
        {
            PrecioTotal -= precioUnidad * linea.Cantidad;
        }
    }

    // Vacía todas las líneas seleccionadas y reinicia el precio total a 0
    public void ClearEncargoCart()
    {
        Encargo.LineasEncargo.Clear();
        PrecioTotal = 0;
    }

    // Reinicia el encargo y el total cuando ya se ha completado y guardado el encargo
    public void EncargoProcessed()
    {
        Encargo = new EncargoImpresionForCreateDTO
        {
            LineasEncargo = new List<LineaEncargoForCreateDTO>()
        };
        PrecioTotal = 0;
    }
}
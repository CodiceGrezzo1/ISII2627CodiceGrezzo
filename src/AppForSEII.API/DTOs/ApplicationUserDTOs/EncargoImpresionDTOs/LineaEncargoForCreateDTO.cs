namespace AppForSEII.API.DTOs.EncargoImpresionDTOs;

public class LineaEncargoForCreateDTO
{
    public int Cantidad { get; set; }
    public int Pieza3DID { get; set; }
    public int MaterialId { get; set; }

    public LineaEncargoForCreateDTO(int cantidad, int pieza3DID, int materialId)
    {
        Cantidad = cantidad;
        Pieza3DID = pieza3DID;
        MaterialId = materialId;
    }

    public override bool Equals(object? obj)
    {
        return obj is LineaEncargoForCreateDTO dto &&
               Cantidad == dto.Cantidad &&
               Pieza3DID == dto.Pieza3DID &&
               MaterialId == dto.MaterialId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Cantidad, Pieza3DID, MaterialId);
    }
}
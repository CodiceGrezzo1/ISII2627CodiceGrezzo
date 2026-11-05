using AppForSEII.API.DTOs.ReservaDTOs;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(ApplicationDbContext context, ILogger<ReservasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Devuelve el detalle de una reserva (equivalente a GetRental)
    // GET api/Reservas/GetReserva?id=1
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(ReservaDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetReserva(int id)
    {
        // Con Select no hace falta Include: EF genera el JOIN con Lineas e Impresora3D
        var reserva = await _context.ReservasImpresora
            .Where(r => r.Id == id)
            .Select(r => new ReservaDetailDTO(
                r.Id,
                r.FechaReserva,
                r.NombreCliente,
                r.ApellidosCliente,
                r.DireccionFacturacion,
                r.PrecioTotal,
                r.MetodoPago,
                r.Lineas
                    .Select(l => new LineaReservaDTO(
                        l.Impresora3D.Id,
                        l.Impresora3D.Nombre,
                        l.Impresora3D.Modelo,
                        l.TiempoReserva,
                        l.PrecioSubtotal))
                    .ToList()))
            .FirstOrDefaultAsync();

        if (reserva == null)
        {
            _logger.LogError($"{DateTime.Now} Error: la reserva con id {id} no existe");
            return NotFound();
        }

        return Ok(reserva);
    }
}
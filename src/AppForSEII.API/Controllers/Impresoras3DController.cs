using AppForSEII.API.DTOs.Impresora3DDTOs;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Impresoras3DController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<Impresoras3DController> _logger;

    public Impresoras3DController(ApplicationDbContext context, ILogger<Impresoras3DController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Flujo principal (paso 2) y flujo alternativo 2.2 (filtrar por nombre y tipo)
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<ImpresoraParaReservarDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetImpresorasParaReservar(string? nombre, TipoImpresora? tipo)
    {
        var impresoras = await _context.Impresoras3D
            .Where(i => (nombre == null || i.Nombre.Contains(nombre))
                     && (tipo == null || i.Tipo == tipo))
            .OrderBy(i => i.Nombre)
            .Select(i => new ImpresoraParaReservarDTO(i.Id, i.Nombre, i.Modelo, i.Tipo,
                i.Descripcion, i.PrecioKilovatioHora, i.PrecioReserva))
            .ToListAsync();

        if (impresoras.Count == 0)
        {
            _logger.LogInformation($"{DateTime.Now} No hay impresoras que cumplan los filtros");
        }

        return Ok(impresoras);
    }
}
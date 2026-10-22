using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.Accesorio3DDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Accesorio3DController : ControllerBase
{
    // Para acceder a la base de datos
    private readonly ApplicationDbContext _context;
    // Para registrar información mientras el sistema se ejecuta
    private readonly ILogger<Accesorio3DController> _logger;

    public Accesorio3DController(ApplicationDbContext context, ILogger<Accesorio3DController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<Accesorio3DForGetDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetAccesorios3D(string? nombre)
    {
        var accesorios = await _context.Accesorios
            .Where(a => (nombre == null) || a.Nombre.Contains(nombre))
            .OrderBy(a => a.Nombre)
            .Select(a => new Accesorio3DForGetDTO(a.Id, a.Nombre, a.Precio, a.Categoria))
            .ToListAsync();

        return Ok(accesorios);
    }
}
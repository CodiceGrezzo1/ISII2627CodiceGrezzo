using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.ApplicationUserDTOs.Accesorio3DDTOs;
using AppForSEII.API.Models;
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
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GetAccesorios3D(string? nombre, string? categoria)
    {
        // Convertimos el texto de la categoría al enum (si el cliente ha indicado una)
        CategoriaAccesorio? categoriaFiltro = null;
        if (!string.IsNullOrWhiteSpace(categoria))
        {
            if (!Enum.TryParse<CategoriaAccesorio>(categoria, true, out var categoriaParseada)
                || !Enum.IsDefined(categoriaParseada))
            {
                _logger.LogError($"{DateTime.Now} Exception: categoría '{categoria}' no válida");
                return BadRequest("Categoría no válida. Valores permitidos: "
                    + string.Join(", ", Enum.GetNames<CategoriaAccesorio>()));
            }
            categoriaFiltro = categoriaParseada;
        }

        var accesorios = await _context.Accesorios
            .Where(a => (nombre == null) || a.Nombre.Contains(nombre))               // filtro por nombre
            .Where(a => (categoriaFiltro == null) || a.Categoria == categoriaFiltro) // filtro por categoría
            .OrderBy(a => a.Nombre)
            .Select(a => new Accesorio3DForGetDTO(
                a.Id, a.Nombre, a.Categoria, a.Compatibilidad, a.CantidadDisponible, a.Precio))
            .ToListAsync();

        return Ok(accesorios); 
    }
}
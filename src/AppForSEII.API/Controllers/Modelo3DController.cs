// Importamos las librerías necesarias de ASP.NET Core y Entity Framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net; // Necesario para HttpStatusCode.OK
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.API.Controllers;

// URL base del controlador: /api/Modelo3D
[Route("api/[controller]")]
[ApiController]
public class Modelo3DController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly ILogger<Modelo3DController> _logger;

    public Modelo3DController(ApplicationDbContext context, ILogger<Modelo3DController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<Modelo3DForListDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetModelos3D(string? name, string? categoria, FormatoModelo3D? formato)
    {
        var modelos = await _context.Modelos3D
            .Where(m => (name == null || m.Nombre.Contains(name)) &&
                        (categoria == null || m.Categoria == categoria) &&
                        (formato == null || m.Formato == formato))
            .OrderBy(m => m.Categoria)   
            .ThenBy(m => m.Nombre)      
            .Select(m => new Modelo3DForListDTO(m.Id, m.Nombre, m.Categoria, m.Formato.ToString(), m.Precio))
            .ToListAsync();

        return Ok(modelos);
    }
}
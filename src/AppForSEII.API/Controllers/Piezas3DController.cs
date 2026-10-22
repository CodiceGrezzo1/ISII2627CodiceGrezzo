// Importamos las librerías necesarias de ASP.NET Core y Entity Framework
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net; // Necesario para HttpStatusCode.OK
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.Pieza3DDTOs;

// El namespace agrupa lógicamente este controlador dentro de la estructura del proyecto
namespace AppForSEII.API.Controllers;

// [Route("api/[controller]")] define la URL base para acceder a este controlador (ej: /api/Piezas3D)[cite: 15]
[Route("api/[controller]")]
// [ApiController] indica que esta clase es un controlador de API, activando comportamientos automáticos[cite: 15]
[ApiController]
public class Piezas3DController : ControllerBase
{
    // Variable para acceder a la base de datos a través de Entity Framework[cite: 15, 17]
    private readonly ApplicationDbContext _context;
    
    // Variable para registrar (loguear) errores o información útil en la consola[cite: 15, 17]
    private readonly ILogger<Piezas3DController> _logger;

    // Constructor del controlador: recibe automáticamente las dependencias (Inyección de Dependencias)[cite: 17]
    public Piezas3DController(ApplicationDbContext context, ILogger<Piezas3DController> logger)
    {
        _context = context; // Guardamos la conexión a la base de datos[cite: 17]
        _logger = logger;   // Guardamos el sistema de logs[cite: 17]
    }

    // ==========================================================================
    // MÉTODO DE ACCIÓN GET: Filtrar Piezas 3D por nombre (o devolver todas si es null)
    // ==========================================================================
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<Pieza3DForListDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetPiezas3D(string? name, CategoriaPieza? categoria)
    {
        // 1. Consultamos la base de datos aplicando el filtro Where y la proyección Select[cite: 29, 30]
        var piezas = await _context.Piezas3D
           .Where(p => (name == null || p.Nombre.Contains(name)) &&
                    (categoria == null || p.Categoria == categoria))
             .OrderBy(p => p.Categoria)     // Orden principal por categoría
            .ThenBy(p => p.Nombre)          // Orden secundario por nombre       
            .Select(p => new Pieza3DForListDTO(p.Id, p.Nombre, p.Peso, p.Categoria.ToString())) // Proyectamos al DTO[cite: 29]
            .ToListAsync();

        // 2. Devolvemos la lista resultante con un código HTTP 200 (OK)[cite: 25, 30]
        return Ok(piezas);
    }

} // <--- Esta es la única llave final que cierra la clase Piezas3DController
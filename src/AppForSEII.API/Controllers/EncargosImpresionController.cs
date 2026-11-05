
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.EncargoImpresionDTOs;
using AppForSEII.API.DTOs.Pieza3DDTOs;


namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EncargosImpresionController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EncargosImpresionController> _logger;

    public EncargosImpresionController(
        ApplicationDbContext context,
        ILogger<EncargosImpresionController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<EncargoImpresionForGetDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetEncargosImpresion(string? nombre, string? apellidos)
    {
        var encargos = await _context.EncargoImpresiones
            .Where(e => (nombre == null || e.NombreCliente.Contains(nombre)) &&
                        (apellidos == null || e.ApellidosCliente.Contains(apellidos)))
            .OrderBy(e => e.ApellidosCliente)
            .ThenBy(e => e.NombreCliente)
            .Select(e => new EncargoImpresionForGetDTO(
                e.Id,
                e.FechaEncargo,
                e.NombreCliente,
                e.ApellidosCliente,
                e.DireccionEnvio,
                e.NumeroTelefono,
                e.Descripcion,
                e.PrecioTotal,
                e.MetodoPago.ToString()
            ))
            .ToListAsync();

        return Ok(encargos);
    }

    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(EncargoImpresionForGetDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetEncargoImpresionDetails(int id)
    {
        var encargo = await _context.EncargoImpresiones
            .Include(e => e.LineasEncargo)
                .ThenInclude(l => l.Pieza3D)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (encargo == null)
        {
            return NotFound();
        }

        var encargoDTO = new EncargoImpresionForGetDTO(
            encargo.Id,
            encargo.FechaEncargo,
            encargo.NombreCliente,
            encargo.ApellidosCliente,
            encargo.DireccionEnvio,
            encargo.NumeroTelefono,
            encargo.Descripcion,
            encargo.PrecioTotal,
            encargo.MetodoPago.ToString()
        );

        encargoDTO.LineasEncargo = encargo.LineasEncargo
            .Select(l => new LineaEncargoForGetDTO(
                l.Id,
                l.Cantidad,
                l.PrecioUnidad,
                l.Subtotal,
                new Pieza3DForListDTO(
                    l.Pieza3D.Id,
                    l.Pieza3D.Nombre,
                    l.Pieza3D.Peso,
                    l.Pieza3D.Categoria.ToString()
                )
            ))
            .ToList();

        return Ok(encargoDTO);
    }
}

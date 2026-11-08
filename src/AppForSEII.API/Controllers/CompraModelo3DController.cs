using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompraModelo3DController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CompraModelo3DController> _logger;

    public CompraModelo3DController(
        ApplicationDbContext context,
        ILogger<CompraModelo3DController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<CompraModelo3DForGetDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetComprasModelo3D(string? nombre, string? apellidos)
    {
        var compras = await _context.ComprasModelo3D
            .Where(c => (nombre == null || c.NombreCliente.Contains(nombre)) &&
                        (apellidos == null || c.ApellidosCliente.Contains(apellidos)))
            .OrderBy(c => c.ApellidosCliente)
            .ThenBy(c => c.NombreCliente)
            .Select(c => new CompraModelo3DForGetDto(
                c.Id,
                c.FechaCompra,
                c.NombreCliente,
                c.ApellidosCliente,
                c.CorreoElectronico,
                c.DireccionFacturacion,
                c.Descripcion,
                c.PrecioTotal,
                c.MetodoPago.ToString()
            ))
            .ToListAsync();

        return Ok(compras);
    }

    [HttpGet]
    [Route("[action]/{id}")]
    [ProducesResponseType(typeof(CompraModelo3DForGetDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetCompraModelo3DDetails(int id)
    {
        var compra = await _context.ComprasModelo3D
            .Include(c => c.LineasCompraModelo)
                .ThenInclude(l => l.Modelo3D)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (compra == null)
        {
            return NotFound();
        }

        var compraDTO = new CompraModelo3DForGetDto(
            compra.Id,
            compra.FechaCompra,
            compra.NombreCliente,
            compra.ApellidosCliente,
            compra.CorreoElectronico,
            compra.DireccionFacturacion,
            compra.Descripcion,
            compra.PrecioTotal,
            compra.MetodoPago.ToString()
        );

        compraDTO.LineasCompraModelo = compra.LineasCompraModelo
            .Select(l => new LineaCompraModeloForGetDto(
                l.Id,
                l.CantidadLicencias,
                l.PrecioUnidad,
                l.Subtotal,
                new Modelo3DForListDTO(
                    l.Modelo3D.Id,
                    l.Modelo3D.Nombre,
                    l.Modelo3D.Categoria,
                    l.Modelo3D.Formato.ToString(),
                    l.Modelo3D.Precio
                )
            ))
            .ToList();

        return Ok(compraDTO);
    }
}

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



    
// MÉTODO POST
    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(CompraModelo3DForGetDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreateCompraModelo3D(CompraModelo3DForCreateDTO createDTO)
    {
        var cliente = await _context.Users.OfType<Cliente>().FirstOrDefaultAsync(c => c.UserName == User.Identity!.Name);
        if (cliente == null)
        {
            ModelState.AddModelError("Cliente", "Error! Client is not registered or authenticated.");
        }

        if (ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }
        if (createDTO.NombreCliente == "Cliente")
        {
            ModelState.AddModelError("NombreCliente","Error!Cliente no es un nombre valido");
        }
        // 1. Declaramos la compra previamente para asociarla en las líneas (evita errores de validación previa)
        var compra = new CompraModelo3D
        {
            FechaCompra = DateTime.Now,
            NombreCliente = createDTO.NombreCliente,
            ApellidosCliente = createDTO.ApellidosCliente,
            CorreoElectronico = createDTO.CorreoElectronico,
            DireccionFacturacion = createDTO.DireccionFacturacion,
            Descripcion = createDTO.Descripcion,
            MetodoPago = createDTO.MetodoPago,
            PrecioTotal = 0,
            Cliente = cliente!,
            LineasCompraModelo = new List<LineaCompraModelo>()
        };

        decimal precioTotalCalculado = 0;

        // 2. Recorremos las líneas de compra para verificar el modelo y calcular los precios y subtotales
        foreach (var item in createDTO.LineasCompraModelo)
        {
            var modelo3D = await _context.Modelos3D.FindAsync(item.Modelo3DId);
            if (modelo3D == null)
            {
                ModelState.AddModelError("LineasCompraModelo", "Error! El modelo 3D indicado no existe.");
                continue;
            }

            decimal precioUnidad = modelo3D.Precio;
            decimal subtotal = precioUnidad * item.CantidadLicencias;

            var linea = new LineaCompraModelo
            {
                CantidadLicencias = item.CantidadLicencias,
                PrecioUnidad = precioUnidad,
                Subtotal = subtotal,
                Modelo3DId = item.Modelo3DId,
                Modelo3D = modelo3D,
                CompraModelo3D = compra
            };

            compra.LineasCompraModelo.Add(linea);
            precioTotalCalculado += subtotal;
        }

        if (ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        // 3. Asignamos el precio total calculado
        compra.PrecioTotal = precioTotalCalculado;

        // 4. Guardamos en la base de datos
        _context.Add(compra);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Conflict("Error: " + ex.Message);
        }

        // Devolvemos el DTO de GET correspondiente para que la respuesta coincida con la estructura esperada
        var compraGetDTO = new CompraModelo3DForGetDto(
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

        compraGetDTO.LineasCompraModelo = compra.LineasCompraModelo
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

        return CreatedAtAction(nameof(GetCompraModelo3DDetails), new { id = compra.Id }, compraGetDTO);
    }

}

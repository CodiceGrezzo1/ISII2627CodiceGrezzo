
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
    //METODO POST
  [HttpPost]
[Route("[action]")]
[ProducesResponseType(typeof(EncargoImpresionForGetDTO), (int)HttpStatusCode.Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
public async Task<ActionResult> CreateEncargoImpresion(EncargoImpresionForCreateDTO createDTO)
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

    // 1. PRIMERO: Declaramos el encargo arriba con la lista vacía 
    // (para que ya exista antes del bucle y solucione el error CS9035)
    var encargo = new EncargoImpresion
    {
        FechaEncargo = DateTime.Now,
        NombreCliente = createDTO.NombreCliente,
        ApellidosCliente = createDTO.ApellidosCliente,
        DireccionEnvio = createDTO.DireccionEnvio,
        NumeroTelefono = createDTO.NumeroTelefono,
        Descripcion = createDTO.Descripcion,
        MetodoPago = createDTO.MetodoPago,
        PrecioTotal = 0,
        Cliente = cliente!,
        LineasEncargo = new List<LineaEncargo>()
    };

    decimal precioTotalCalculado = 0;

    // 2. SEGUNDO: Recorremos las líneas para calcular y asociarlas
    foreach (var item in createDTO.LineasEncargo)
    {
        var material = await _context.Materiales.FindAsync(item.MaterialId);
        var pieza = await _context.Piezas3D.FindAsync(item.Pieza3DID);

        if (material == null || pieza == null)
        {
            ModelState.AddModelError("LineasEncargo", "Error! La pieza o el material indicados no existen.");
            continue;
        }

        decimal precioUnidad = material.PrecioPorGramo; // Propiedad correcta del Material
        decimal subtotal = precioUnidad * item.Cantidad;

        var linea = new LineaEncargo
        {
            Cantidad = item.Cantidad,
            PrecioUnidad = precioUnidad,
            Subtotal = subtotal,
            Pieza3DId = item.Pieza3DID, // 'd' minúscula según tu modelo
            MaterialId = item.MaterialId,
            Pieza3D = pieza,
            Material = material,
            EncargoImpresion = encargo // Satisface la propiedad required
        };

        encargo.LineasEncargo.Add(linea);
        precioTotalCalculado += subtotal;
    }

    if (ModelState.ErrorCount > 0)
    {
        return BadRequest(new ValidationProblemDetails(ModelState));
    }

    // 3. TERCERO: Asignamos el precio total ya calculado al encargo
    encargo.PrecioTotal = precioTotalCalculado;

    // 4. CUARTO: Guardamos en la base de datos
    _context.Add(encargo);

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex.Message);
        return Conflict("Error: " + ex.Message);
    }

    return Ok(encargo);
}
}
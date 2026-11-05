using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.CompraAccesoriosDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraAccesoriosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompraAccesoriosController> _logger;

        public CompraAccesoriosController(ApplicationDbContext context, ILogger<CompraAccesoriosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraAccesoriosForGetDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCompraAccesorios(int id)
        {
            var compra = await _context.CompraAccesorios
                .Where(c => c.Id == id)
                .Select(c => new CompraAccesoriosForGetDTO(c.Id, c.FechaCompra, c.Cliente.UserName!,
                    c.NombreCliente, c.ApellidosCliente, c.DireccionEnvio, c.NumeroTelefono, c.MetodoPago,
                    c.LineasCompraAccesorio.Select(l => new LineaCompraAccesorioDTO(l.Accesorio.Id,
                        l.NombreAccesorio, l.Cantidad, l.PrecioUnitario)).ToList()))
                .FirstOrDefaultAsync();

            if (compra == null)
            {
                _logger.LogError($"Error: CompraAccesorios con id {id} no existe");
                return NotFound();
            }

            return Ok(compra);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraAccesoriosForGetDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateCompraAccesorios(CompraAccesoriosForCreateDTO compraDTO)
        {
            // Los atributos del DTO ([Required], [Range]...) ya se validan antes de entrar aquí
            if (compraDTO.Lineas.Count == 0)
                ModelState.AddModelError("Lineas", "Error! Debes incluir al menos un accesorio en la compra");

            var cliente = _context.Users.OfType<Cliente>()
                .FirstOrDefault(c => c.UserName == compraDTO.UserNameCliente);
            if (cliente == null)
                ModelState.AddModelError("Cliente", "Error! El nombre de usuario no está registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var ids = compraDTO.Lineas.Select(l => l.AccesorioId).ToList();
            var accesorios = _context.Accesorios.Where(a => ids.Contains(a.Id)).ToList();

            var compra = new CompraAccesorios
            {
                FechaCompra = DateTime.Now,
                NombreCliente = compraDTO.NombreCliente,
                ApellidosCliente = compraDTO.ApellidosCliente,
                DireccionEnvio = compraDTO.DireccionEnvio,
                NumeroTelefono = compraDTO.NumeroTelefono,
                MetodoPago = compraDTO.MetodoPago,
                Cliente = cliente!
            };

            foreach (var linea in compraDTO.Lineas)
            {
                var accesorio = accesorios.FirstOrDefault(a => a.Id == linea.AccesorioId);
                if (accesorio == null)
                {
                    ModelState.AddModelError("Lineas", $"Error! El accesorio con id {linea.AccesorioId} no existe");
                }
                else if (linea.Cantidad > accesorio.CantidadDisponible)
                {
                    ModelState.AddModelError("Lineas", $"Error! No hay stock suficiente del accesorio '{accesorio.Nombre}'");
                }
                else
                {
                    // Accesorio y CompraAccesorios son "required": hay que asignar los dos
                    compra.LineasCompraAccesorio.Add(new LineaCompraAccesorio
                    {
                        NombreAccesorio = accesorio.Nombre,
                        Cantidad = linea.Cantidad,
                        PrecioUnitario = accesorio.Precio,
                        Accesorio = accesorio,
                        CompraAccesorios = compra
                    });
                    accesorio.CantidadDisponible -= linea.Cantidad;   // descuenta el stock
                    linea.NombreAccesorio = accesorio.Nombre;
                    linea.PrecioUnitario = accesorio.Precio;
                }
            }

            compra.PrecioTotal = compra.LineasCompraAccesorio.Sum(l => l.Subtotal);

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            _context.Add(compra);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Conflict("Error " + ex.Message);
            }

            var detalle = new CompraAccesoriosForGetDTO(compra.Id, compra.FechaCompra, compraDTO.UserNameCliente,
                compra.NombreCliente, compra.ApellidosCliente, compra.DireccionEnvio, compra.NumeroTelefono,
                compra.MetodoPago, compraDTO.Lineas);

            return CreatedAtAction("GetCompraAccesorios", new { id = compra.Id }, detalle);
        }
    }
}
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
    }
}
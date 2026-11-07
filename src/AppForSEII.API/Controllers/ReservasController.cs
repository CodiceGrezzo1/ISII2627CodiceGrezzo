using AppForSEII.API.DTOs.ReservaDTOs;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(ApplicationDbContext context, ILogger<ReservasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Devuelve el detalle de una reserva (equivalente a GetRental)
    // GET api/Reservas/GetReserva?id=1
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(ReservaDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetReserva(int id)
    {
        // Con Select no hace falta Include: EF genera el JOIN con Lineas e Impresora3D
        var reserva = await _context.ReservasImpresora
            .Where(r => r.Id == id)
            .Select(r => new ReservaDetailDTO(
                r.Id,
                r.FechaReserva,
                r.NombreCliente,
                r.ApellidosCliente,
                r.DireccionFacturacion,
                r.PrecioTotal,
                r.MetodoPago,
                r.Lineas
                    .Select(l => new LineaReservaDTO(
                        l.Impresora3D.Id,
                        l.Impresora3D.Nombre,
                        l.Impresora3D.Modelo,
                        l.Impresora3D.Tipo,
                        l.Impresora3D.Descripcion,
                        l.Impresora3D.PrecioKilovatioHora,
                        l.Impresora3D.PrecioReserva,
                        l.TiempoReserva,
                        l.PrecioSubtotal))
                    .ToList()))
            .FirstOrDefaultAsync();

        if (reserva == null)
        {
            _logger.LogError($"{DateTime.Now} Error: la reserva con id {id} no existe");
            return NotFound();
        }

        return Ok(reserva);
    }

    // Crea una reserva de impresoras (equivalente a CreateRental)
    // POST api/Reservas/CreateReserva
    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(ReservaDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> CreateReserva(ReservaForCreateDTO reservaForCreate)
    {
        // Las validaciones definidas con DataAnnotations en ReservaForCreateDTO (datos obligatorios,
        // flujo alternativo 7.1) las comprueba [ApiController] antes de entrar en el método.

        // Flujo alternativo 5: no se puede reservar sin ninguna impresora
        if (reservaForCreate.Lineas.Count == 0)
            ModelState.AddModelError("Lineas", "Error! Debe incluir al menos una impresora para reservar");

        // El cliente debe estar registrado como Cliente
        var cliente = await _context.ApplicationUsers.OfType<Cliente>()
            .FirstOrDefaultAsync(c => c.UserName == reservaForCreate.ClienteUserName);
        if (cliente == null)
            ModelState.AddModelError("Cliente", "Error! El usuario no está registrado como cliente");

        // Una impresora solo puede aparecer una vez en la reserva
        var idsSolicitados = reservaForCreate.Lineas.Select(l => l.ImpresoraId).ToList();
        if (idsSolicitados.Count != idsSolicitados.Distinct().Count())
            ModelState.AddModelError("Lineas", "Error! No se puede reservar la misma impresora más de una vez en la misma reserva");

        var impresoras = await _context.Impresoras3D
            .Where(i => idsSolicitados.Contains(i.Id))
            .ToListAsync();

        foreach (var linea in reservaForCreate.Lineas)
        {
            // Flujo alternativo 7.3: solo son válidos los tiempos definidos en TiempoReserva (1 a 5 horas, 1 día, 2 días)
            if (!Enum.IsDefined(linea.TiempoReserva))
                ModelState.AddModelError("Lineas", $"Error! El tiempo de reserva de la impresora con id {linea.ImpresoraId} no es válido");

            // Flujo alternativo 7.4: alguna de las impresoras seleccionadas ya no existe
            if (!impresoras.Any(i => i.Id == linea.ImpresoraId))
                ModelState.AddModelError("Lineas", $"Error! La impresora con id {linea.ImpresoraId} no existe o ya no está disponible");
        }

        if (ModelState.ErrorCount > 0)
            return BadRequest(new ValidationProblemDetails(ModelState));

        var reserva = new ReservaImpresora
        {
            FechaReserva = DateTime.Now,
            NombreCliente = reservaForCreate.NombreCliente,
            ApellidosCliente = reservaForCreate.ApellidosCliente,
            DireccionFacturacion = reservaForCreate.DireccionFacturacion,
            MetodoPago = reservaForCreate.MetodoPago,
            ClienteId = cliente!.Id,
            Cliente = cliente
        };

        foreach (var linea in reservaForCreate.Lineas)
        {
            var impresora = impresoras.First(i => i.Id == linea.ImpresoraId);

            // Se usa el inicializador de objeto porque LineaReserva tiene miembros 'required'
            reserva.Lineas.Add(new LineaReserva
            {
                TiempoReserva = linea.TiempoReserva,
                PrecioSubtotal = CalcularSubtotal(impresora, linea.TiempoReserva),
                Impresora3D = impresora,
                ReservaImpresora = reserva
            });
        }

        reserva.PrecioTotal = reserva.Lineas.Sum(l => l.PrecioSubtotal);

        _context.Add(reserva);

        try
        {
            // Se guardan en la base de datos la reserva y sus líneas
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Conflict("Error" + ex.Message);
        }

        var reservaDetail = new ReservaDetailDTO(
            reserva.Id,
            reserva.FechaReserva,
            reserva.NombreCliente,
            reserva.ApellidosCliente,
            reserva.DireccionFacturacion,
            reserva.PrecioTotal,
            reserva.MetodoPago,
            reserva.Lineas
                .Select(l => new LineaReservaDTO(
                    l.Impresora3D.Id,
                    l.Impresora3D.Nombre,
                    l.Impresora3D.Modelo,
                    l.Impresora3D.Tipo,
                    l.Impresora3D.Descripcion,
                    l.Impresora3D.PrecioKilovatioHora,
                    l.Impresora3D.PrecioReserva,
                    l.TiempoReserva,
                    l.PrecioSubtotal))
                .ToList());

        return CreatedAtAction("GetReserva", new { id = reserva.Id }, reservaDetail);
    }

    
    private static decimal CalcularSubtotal(Impresora3D impresora, TiempoReserva tiempo)
    {
        return impresora.PrecioReserva * HorasDeReserva(tiempo);
    }

    private static int HorasDeReserva(TiempoReserva tiempo)
    {
        return tiempo switch
        {
            TiempoReserva.UnaHora => 1,
            TiempoReserva.DosHoras => 2,
            TiempoReserva.TresHoras => 3,
            TiempoReserva.CuatroHoras => 4,
            TiempoReserva.CincoHoras => 5,
            TiempoReserva.UnDia => 24,
            TiempoReserva.DosDias => 48,
            _ => throw new ArgumentOutOfRangeException(nameof(tiempo), tiempo, "Tiempo de reserva no válido")
        };
    }
}
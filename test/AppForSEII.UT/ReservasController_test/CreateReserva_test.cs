using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ReservaDTOs;

namespace AppForSEII.UT.ReservasController_test;

// Complejidad ciclomática de CreateReserva = 10 (9 puntos de decisión + 1):
//   if Lineas vacías, if cliente nulo, if impresora repetida, foreach de validación, if tiempo no válido,
//   if impresora inexistente, if hay errores, foreach de creación y catch de SaveChangesAsync.
// Cada decisión se prueba en sus dos sentidos con los casos de abajo:
//   - Fact  Success      -> camino sin errores
//   - Theory Subtotal    -> los 7 valores de TiempoReserva (switch de horas)
//   - Theory Errores     -> los 6 casos de error de validación (400)
//   - Fact  Conflict     -> fallo al guardar (409)
public class CreateReserva_test : AppForSEII4SqliteUT
{
    private const string ClienteUserName = "elena@uclm.es";
    // Ids fijos para poder usarlos también en el MemberData (que es estático)
    private const int Impresora1Id = 1; // PrecioReserva 2.50
    private const int Impresora2Id = 2; // PrecioReserva 4.00

    public CreateReserva_test()
    {
        // Datos de partida: un cliente y dos impresoras
        var cliente = new Cliente("1", "Elena", "Navarro Martínez", ClienteUserName, "Calle Mayor 1, Albacete");
        var impresora1 = new Impresora3D
        {
            Id = Impresora1Id,
            Nombre = "Ender 3",
            Modelo = "V2",
            Tipo = TipoImpresora.Filamento,
            Descripcion = "Impresora de filamento",
            PrecioKilovatioHora = 0.15m,
            PrecioReserva = 2.50m
        };
        var impresora2 = new Impresora3D
        {
            Id = Impresora2Id,
            Nombre = "Anycubic Photon",
            Modelo = "Mono X",
            Tipo = TipoImpresora.Resina,
            Descripcion = "Impresora de resina",
            PrecioKilovatioHora = 0.20m,
            PrecioReserva = 4.00m
        };

        _context.AddRange(cliente, impresora1, impresora2);
        _context.SaveChanges();
    }

    private static ReservaForCreateDTO CrearReserva(params LineaReservaForCreateDTO[] lineas)
    {
        return new ReservaForCreateDTO(ClienteUserName, "Elena", "Navarro Martínez",
            "Calle Mayor 1, Albacete", MetodoPago.paypal, lineas.ToList());
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task CreateReserva_Success_test()
    {
        // Arrange
        var mock = new Mock<ILogger<ReservasController>>();
        var controller = new ReservasController(_context, mock.Object);
        var reservaForCreate = CrearReserva(
            new LineaReservaForCreateDTO(Impresora1Id, TiempoReserva.UnaHora),
            new LineaReservaForCreateDTO(Impresora2Id, TiempoReserva.TresHoras));
        var antes = DateTime.Now;

        // Act
        var result = await controller.CreateReserva(reservaForCreate);
        var despues = DateTime.Now;

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetReserva", createdResult.ActionName);

        var detalle = Assert.IsType<ReservaDetailDTO>(createdResult.Value);
        Assert.Equal(detalle.Id, Assert.IsType<int>(createdResult.RouteValues!["id"]));
        Assert.InRange(detalle.FechaReserva, antes, despues);
        Assert.Equal("Elena", detalle.NombreCliente);
        Assert.Equal("Navarro Martínez", detalle.ApellidosCliente);
        Assert.Equal("Calle Mayor 1, Albacete", detalle.DireccionFacturacion);
        Assert.Equal(MetodoPago.paypal, detalle.MetodoPago);
        // 1 hora x 2.50 + 3 horas x 4.00
        Assert.Equal(14.50m, detalle.PrecioTotal);
        Assert.Equal(2, detalle.Lineas.Count);

        // La reserva y sus líneas se han guardado en la base de datos
        using var contextoNuevo = CreateContext();
        var guardada = await contextoNuevo.ReservasImpresora.Include(r => r.Lineas).SingleAsync();
        Assert.Equal("1", guardada.ClienteId);
        Assert.Equal(14.50m, guardada.PrecioTotal);
        Assert.Equal(2, guardada.Lineas.Count);
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    [InlineData(TiempoReserva.UnaHora, 1)]
    [InlineData(TiempoReserva.DosHoras, 2)]
    [InlineData(TiempoReserva.TresHoras, 3)]
    [InlineData(TiempoReserva.CuatroHoras, 4)]
    [InlineData(TiempoReserva.CincoHoras, 5)]
    [InlineData(TiempoReserva.UnDia, 24)]
    [InlineData(TiempoReserva.DosDias, 48)]
    public async Task CreateReserva_Subtotal_test(TiempoReserva tiempo, int horasEsperadas)
    {
        // Arrange
        var mock = new Mock<ILogger<ReservasController>>();
        var controller = new ReservasController(_context, mock.Object);
        var reservaForCreate = CrearReserva(new LineaReservaForCreateDTO(Impresora1Id, tiempo));
        var subtotalEsperado = 2.50m * horasEsperadas;

        // Act
        var result = await controller.CreateReserva(reservaForCreate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var detalle = Assert.IsType<ReservaDetailDTO>(createdResult.Value);
        var linea = Assert.Single(detalle.Lineas);
        Assert.Equal(tiempo, linea.TiempoReserva);
        Assert.Equal(subtotalEsperado, linea.PrecioSubtotal);
        Assert.Equal(subtotalEsperado, detalle.PrecioTotal);
    }

    public static IEnumerable<object[]> ErroresDeValidacion()
    {






        yield return new object[]
        {
            new ReservaForCreateDTO(ClienteUserName, "Al", "Navarro Martinez", "Calle MAyor1",
             MetodoPago.paypal, new List<LineaReservaForCreateDTO> { new LineaReservaForCreateDTO(Impresora1Id, TiempoReserva.UnaHora)}),
            "NombreCliente", "El nombre del cliente debe de tener minimo 3 caracteres"
        };









        // Flujo alternativo 5: ninguna impresora seleccionada
        yield return new object[]
        {
            CrearReserva(),
            "Lineas",
            "Error! Debe incluir al menos una impresora para reservar"
        };

        // El usuario no está registrado como cliente
        yield return new object[]
        {
            new ReservaForCreateDTO("nadie@uclm.es", "Elena", "Navarro Martínez", "Calle Mayor 1",
                MetodoPago.paypal, new List<LineaReservaForCreateDTO> { new LineaReservaForCreateDTO(Impresora1Id, TiempoReserva.UnaHora) }),
            "Cliente",
            "Error! El usuario no está registrado como cliente"
        };

        // La misma impresora dos veces en la misma reserva
        yield return new object[]
        {
            CrearReserva(
                new LineaReservaForCreateDTO(Impresora1Id, TiempoReserva.UnaHora),
                new LineaReservaForCreateDTO(Impresora1Id, TiempoReserva.DosHoras)),
            "Lineas",
            "Error! No se puede reservar la misma impresora más de una vez en la misma reserva"
        };

        // Flujo alternativo 7.3: tiempo de reserva que no está entre los válidos
        yield return new object[]
        {
            CrearReserva(new LineaReservaForCreateDTO(Impresora1Id, (TiempoReserva)99)),
            "Lineas",
            "Error! El tiempo de reserva de la impresora con id 1 no es válido"
        };

        // Flujo alternativo 7.4: la impresora seleccionada no existe
        yield return new object[]
        {
            CrearReserva(new LineaReservaForCreateDTO(999, TiempoReserva.UnaHora)),
            "Lineas",
            "Error! La impresora con id 999 no existe o ya no está disponible"
        };

        // Una impresora válida y otra que no existe: no se guarda nada
        yield return new object[]
        {
            CrearReserva(
                new LineaReservaForCreateDTO(Impresora1Id, TiempoReserva.UnaHora),
                new LineaReservaForCreateDTO(999, TiempoReserva.UnaHora)),
            "Lineas",
            "Error! La impresora con id 999 no existe o ya no está disponible"
        };
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    [MemberData(nameof(ErroresDeValidacion))]
    public async Task CreateReserva_Errores_test(ReservaForCreateDTO reservaForCreate, string claveEsperada, string mensajeEsperado)
    {
        // Arrange
        var mock = new Mock<ILogger<ReservasController>>();
        var controller = new ReservasController(_context, mock.Object);

        // Act
        var result = await controller.CreateReserva(reservaForCreate);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var problemas = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        Assert.True(problemas.Errors.ContainsKey(claveEsperada));
        Assert.Contains(mensajeEsperado, problemas.Errors[claveEsperada]);

        // No se ha guardado nada
        Assert.False(await _context.ReservasImpresora.AnyAsync());
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task CreateReserva_ErrorAlGuardar_Conflict_test()
    {
        // Arrange: un contexto cuyo SaveChangesAsync siempre falla
        var mock = new Mock<ILogger<ReservasController>>();
        using var contextoQueFalla = new ContextoQueFallaAlGuardar(_contextOptions);
        var controller = new ReservasController(contextoQueFalla, mock.Object);
        var reservaForCreate = CrearReserva(new LineaReservaForCreateDTO(Impresora1Id, TiempoReserva.UnaHora));

        // Act
        var result = await controller.CreateReserva(reservaForCreate);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        var mensaje = Assert.IsType<string>(conflictResult.Value);
        Assert.StartsWith("Error", mensaje);
    }

    private class ContextoQueFallaAlGuardar : ApplicationDbContext
    {
        public ContextoQueFallaAlGuardar(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            throw new DbUpdateException("Fallo simulado al guardar los cambios");
        }
    }
}
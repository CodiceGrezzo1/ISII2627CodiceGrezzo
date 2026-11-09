using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ReservaDTOs;

namespace AppForSEII.UT.ReservasController_test;

// Complejidad ciclomática de GetReserva = 2 (un único if) -> 2 pruebas: reserva inexistente y reserva existente
public class GetReserva_test : AppForSEII4SqliteUT
{
    private readonly int _reservaId;

    public GetReserva_test()
    {
        // Datos de partida: un cliente, una impresora y una reserva con una línea
        var cliente = new Cliente("1", "Elena", "Navarro Martínez", "elena@uclm.es", "Calle Mayor 1, Albacete");
        var impresora = new Impresora3D
        {
            Id = 1,
            Nombre = "Ender 3",
            Modelo = "V2",
            Tipo = TipoImpresora.Filamento,
            Descripcion = "Impresora de filamento",
            PrecioKilovatioHora = 0.15m,
            PrecioReserva = 2.50m
        };
        var reserva = new ReservaImpresora
        {
            FechaReserva = new DateTime(2026, 11, 5, 10, 0, 0),
            NombreCliente = "Elena",
            ApellidosCliente = "Navarro Martínez",
            DireccionFacturacion = "Calle Mayor 1, Albacete",
            PrecioTotal = 5.00m,
            MetodoPago = MetodoPago.paypal,
            ClienteId = cliente.Id,
            Cliente = cliente
        };
        // LineaReserva tiene miembros 'required', por eso se usa el inicializador de objeto
        reserva.Lineas.Add(new LineaReserva
        {
            TiempoReserva = TiempoReserva.DosHoras,
            PrecioSubtotal = 5.00m,
            Impresora3D = impresora,
            ReservaImpresora = reserva
        });

        _context.AddRange(cliente, impresora);
        _context.Add(reserva);
        _context.SaveChanges();
        _reservaId = reserva.Id;
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task GetReserva_NotFound_test()
    {
        // Arrange
        var mock = new Mock<ILogger<ReservasController>>();
        var controller = new ReservasController(_context, mock.Object);

        // Act
        var result = await controller.GetReserva(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task GetReserva_Found_test()
    {
        // Arrange
        var mock = new Mock<ILogger<ReservasController>>();
        var controller = new ReservasController(_context, mock.Object);

        // Act
        var result = await controller.GetReserva(_reservaId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var detalle = Assert.IsType<ReservaDetailDTO>(okResult.Value);

        Assert.Equal(_reservaId, detalle.Id);
        Assert.Equal(new DateTime(2026, 11, 5, 10, 0, 0), detalle.FechaReserva);
        Assert.Equal("Elena", detalle.NombreCliente);
        Assert.Equal("Navarro Martínez", detalle.ApellidosCliente);
        Assert.Equal("Calle Mayor 1, Albacete", detalle.DireccionFacturacion);
        Assert.Equal(5.00m, detalle.PrecioTotal);
        Assert.Equal(MetodoPago.paypal, detalle.MetodoPago);

        var linea = Assert.Single(detalle.Lineas);
        Assert.Equal(1, linea.ImpresoraId);
        Assert.Equal("Ender 3", linea.ImpresoraNombre);
        Assert.Equal("V2", linea.ImpresoraModelo);
        Assert.Equal(TipoImpresora.Filamento, linea.ImpresoraTipo);
        Assert.Equal("Impresora de filamento", linea.ImpresoraDescripcion);
        Assert.Equal(0.15m, linea.PrecioKilovatioHora);
        Assert.Equal(2.50m, linea.PrecioReserva);
        Assert.Equal(TiempoReserva.DosHoras, linea.TiempoReserva);
        Assert.Equal(5.00m, linea.PrecioSubtotal);
    }
}
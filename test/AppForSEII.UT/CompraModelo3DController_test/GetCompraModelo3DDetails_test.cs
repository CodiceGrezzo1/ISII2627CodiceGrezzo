using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.DTOs.Modelo3DDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.CompraModelo3DController_test;

public class GetCompraModelo3DDetails_test : AppForSEII4SqliteUT
{
    private static readonly DateTime _fecha = new DateTime(2026, 11, 1, 10, 0, 0);

    public GetCompraModelo3DDetails_test()
    {
        var cliente = new Cliente("1", "Daniel", "Gómez", "daniel@test.com", "Calle Falsa 123");
        var licencia = new LicenciaModelo3D("Licencia de prueba", new DateTime(2027, 11, 1));
        var modelo = new Modelo3D("Dragon Articulado", "Miniaturas", FormatoModelo3D.STL, 15.0m, licencia);

        var compra = new CompraModelo3D("Daniel", "Gómez", "daniel@test.com", "Calle Falsa 123", 15.0m,
            MetodoPago.tarjetaCredito, cliente) { Cliente = cliente, FechaCompra = _fecha };
        compra.LineasCompraModelo.Add(new LineaCompraModelo(1, 15.0m, 15.0m, compra, modelo));

        _context.Add(cliente);
        _context.Add(licencia);
        _context.Add(modelo);
        _context.Add(compra);
        _context.SaveChanges();
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetCompraModelo3DDetails_NotFound_test()
    {
        // Arrange
        var mock = new Mock<ILogger<CompraModelo3DController>>();
        ILogger<CompraModelo3DController> logger = mock.Object;
        var controller = new CompraModelo3DController(_context, logger);

        // Act
        var result = await controller.GetCompraModelo3DDetails(0);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetCompraModelo3DDetails_Found_test()
    {
        // Arrange
        var mock = new Mock<ILogger<CompraModelo3DController>>();
        ILogger<CompraModelo3DController> logger = mock.Object;
        var controller = new CompraModelo3DController(_context, logger);

        var expected = new CompraModelo3DForGetDto(1, _fecha, "Daniel", "Gómez", "daniel@test.com",
            "Calle Falsa 123", null, 15.0m, "tarjetaCredito");
        expected.LineasCompraModelo = new List<LineaCompraModeloForGetDto>()
        {
            new LineaCompraModeloForGetDto(1, 1, 15.0m, 15.0m,
                new Modelo3DForListDTO(1, "Dragon Articulado", "Miniaturas", "STL", 15.0m))
        };

        // Act
        var result = await controller.GetCompraModelo3DDetails(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<CompraModelo3DForGetDto>(okResult.Value);
        Assert.Equal(expected, actual);
    }
}
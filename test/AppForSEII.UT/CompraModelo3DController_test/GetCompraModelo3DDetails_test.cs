using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.CompraModelo3DController_test;

public class GetCompraModelo3DDetails_test : AppForSEII4SqliteUT
{
    public GetCompraModelo3DDetails_test()
    {
        var licencia = new LicenciaModelo3D
        {
            Id = 1,
            Nombre = "Licencia de prueba",
            FechaExpiracion = new DateTime(2027, 11, 1)
        };

        var modelo = new Modelo3D
        {
            Id = 1,
            Nombre = "Dragon Articulado",
            Categoria = "Minijaturas",
            Formato = FormatoModelo3D.STL,
            Precio = 15.0m,
            LicenciaModelo3D = licencia
        };

        var cliente = new Cliente
        {
            Id = "cliente-1",
            UserName = "daniel@test.com",
            Email = "daniel@test.com"
        };

        var compra = new CompraModelo3D
        {
            Id = 1,
            FechaCompra = new DateTime(2026, 11, 1, 10, 0, 0),
            NombreCliente = "Daniel",
            ApellidosCliente = "Gómez",
            CorreoElectronico = "daniel@test.com",
            DireccionFacturacion = "Calle Falsa 123",
            MetodoPago = MetodoPago.tarjetaCredito,
            PrecioTotal = 15.0m,
            Cliente = cliente,
            LineasCompraModelo = new List<LineaCompraModelo>
            {
                new LineaCompraModelo { Id = 1, CantidadLicencias = 1, PrecioUnidad = 15.0m, Subtotal = 15.0m, Modelo3D = modelo, Modelo3DId = 1 }
            }
        };

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

        // Act
        var result = await controller.GetCompraModelo3DDetails(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<CompraModelo3DForGetDto>(okResult.Value);
        Assert.Equal(1, actual.Id);
        Assert.Equal("Daniel", actual.NombreCliente);
        Assert.Equal("Gómez", actual.ApellidosCliente);
        Assert.Equal("tarjetaCredito", actual.MetodoPago);
        Assert.Equal(15.0m, actual.PrecioTotal);
        Assert.Single(actual.LineasCompraModelo); // el Include/ThenInclude carga la línea
    }
}
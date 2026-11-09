using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.CompraModelo3DController_test;

public class GetComprasModelo3D_test : AppForSEII4SqliteUT
{
    private static readonly DateTime _fecha = new DateTime(2026, 11, 1, 10, 0, 0);

    public GetComprasModelo3D_test()
    {
        var cliente = new Cliente("1", "Daniel", "Gómez", "daniel@test.com", "Calle Falsa 123");
        var licencia = new LicenciaModelo3D("Licencia de prueba", new DateTime(2027, 11, 1));
        var modelo = new Modelo3D("Dragon Articulado", "Miniaturas", FormatoModelo3D.STL, 15.0m, licencia);

        var compra1 = new CompraModelo3D("Daniel", "Gómez", "daniel@test.com", "Calle Falsa 123", 15.0m,
            MetodoPago.tarjetaCredito, cliente) { Cliente = cliente, FechaCompra = _fecha };
        compra1.LineasCompraModelo.Add(new LineaCompraModelo(1, 15.0m, 15.0m, compra1, modelo));

        var compra2 = new CompraModelo3D("Lucía", "Alonso", "lucia@test.com", "Calle Mayor 5", 30.0m,
            MetodoPago.paypal, cliente) { Cliente = cliente, FechaCompra = _fecha };
        compra2.LineasCompraModelo.Add(new LineaCompraModelo(2, 15.0m, 30.0m, compra2, modelo));

        _context.Add(cliente);
        _context.Add(licencia);
        _context.Add(modelo);
        _context.AddRange(compra1, compra2);
        _context.SaveChanges();
    }

    public static IEnumerable<object?[]> TestCasesFor_GetComprasModelo3D_OK()
    {
        // En el listado las líneas no se cargan, así que el DTO esperado no lleva líneas
        var daniel = new CompraModelo3DForGetDto(1, _fecha, "Daniel", "Gómez", "daniel@test.com",
            "Calle Falsa 123", null, 15.0m, "tarjetaCredito");
        var lucia = new CompraModelo3DForGetDto(2, _fecha, "Lucía", "Alonso", "lucia@test.com",
            "Calle Mayor 5", null, 30.0m, "paypal");

        return new List<object?[]>
        {
            // nombre, apellidos, compras esperadas (ordenadas por apellidos y nombre)
            new object?[] { null, null, new List<CompraModelo3DForGetDto>() { lucia, daniel } },    // caso 1
            new object?[] { "Dan", null, new List<CompraModelo3DForGetDto>() { daniel } },          // caso 2
            new object?[] { null, "Alo", new List<CompraModelo3DForGetDto>() { lucia } },           // caso 3
            new object?[] { "Dan", "Gómez", new List<CompraModelo3DForGetDto>() { daniel } },       // caso 4
            new object?[] { "xyz", null, new List<CompraModelo3DForGetDto>() },                     // caso 5: sin resultados
        };
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_GetComprasModelo3D_OK))]
    public async Task GetComprasModelo3D_OK_test(string? nombre, string? apellidos, List<CompraModelo3DForGetDto> expected)
    {
        // Arrange
        var mock = new Mock<ILogger<CompraModelo3DController>>();
        ILogger<CompraModelo3DController> logger = mock.Object;
        var controller = new CompraModelo3DController(_context, logger);

        // Act
        var result = await controller.GetComprasModelo3D(nombre, apellidos);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<List<CompraModelo3DForGetDto>>(okResult.Value);
        Assert.Equal(expected, actual);
    }
}
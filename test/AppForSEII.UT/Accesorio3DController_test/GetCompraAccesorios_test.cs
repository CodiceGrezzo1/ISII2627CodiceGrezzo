using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraAccesoriosDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.CompraAccesoriosController_test;

public class GetCompraAccesorios_test : AppForSEII4SqliteUT
{
    private const string _userName = "elena@uclm.es";
    private const string _nombre = "Elena";
    private const string _apellidos = "Navarro Martínez";
    private const string _direccion = "Avda. España s/n, Albacete";
    private const string _telefono = "600123456";

    public GetCompraAccesorios_test()
    {
        var cliente = new Cliente("1", _nombre, _apellidos, _userName, _direccion);

        var accesorios = new List<Accesorio>()
        {
            new Accesorio("Boquilla 0.4", CategoriaAccesorio.Boquillas, "Ender 3", 10, 5.0m),
            new Accesorio("Base magnética", CategoriaAccesorio.BasesImpresion, "Universal", 5, 20.0m),
        };

        // Cliente, Accesorio y CompraAccesorios son "required": además del constructor
        // hay que asignarlos en el inicializador
        var compra = new CompraAccesorios(_nombre, _apellidos, _direccion, _telefono, 30.0m,
            MetodoPago.tarjetaCredito, cliente) { Cliente = cliente };

        compra.LineasCompraAccesorio.Add(new LineaCompraAccesorio("Boquilla 0.4", 2, 5.0m, accesorios[0], compra)
            { Accesorio = accesorios[0], CompraAccesorios = compra });
        compra.LineasCompraAccesorio.Add(new LineaCompraAccesorio("Base magnética", 1, 20.0m, accesorios[1], compra)
            { Accesorio = accesorios[1], CompraAccesorios = compra });

        _context.Add(cliente);
        _context.AddRange(accesorios);
        _context.Add(compra);
        _context.SaveChanges();
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetCompraAccesorios_NotFound_test()
    {
        // Arrange
        var mock = new Mock<ILogger<CompraAccesoriosController>>();
        ILogger<CompraAccesoriosController> logger = mock.Object;
        var controller = new CompraAccesoriosController(_context, logger);

        // Act
        var result = await controller.GetCompraAccesorios(0);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetCompraAccesorios_Found_test()
    {
        // Arrange
        var mock = new Mock<ILogger<CompraAccesoriosController>>();
        ILogger<CompraAccesoriosController> logger = mock.Object;
        var controller = new CompraAccesoriosController(_context, logger);

        // FechaCompra no se compara en el Equals del DTO, así que DateTime.Now no afecta
        var expected = new CompraAccesoriosForGetDTO(1, DateTime.Now, _userName, _nombre, _apellidos,
            _direccion, _telefono, MetodoPago.tarjetaCredito,
            new List<LineaCompraAccesorioDTO>()
            {
                new LineaCompraAccesorioDTO(1, "Boquilla 0.4", 2, 5.0m),
                new LineaCompraAccesorioDTO(2, "Base magnética", 1, 20.0m),
            });

        // Act
        var result = await controller.GetCompraAccesorios(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<CompraAccesoriosForGetDTO>(okResult.Value);
        Assert.Equal(expected, actual);
    }
}
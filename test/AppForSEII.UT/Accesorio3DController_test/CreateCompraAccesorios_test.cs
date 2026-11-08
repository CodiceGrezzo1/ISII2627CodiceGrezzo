using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompraAccesoriosDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.CompraAccesoriosController_test;

public class CreateCompraAccesorios_test : AppForSEII4SqliteUT
{
    private const string _userName = "elena@uclm.es";
    private const string _nombre = "Elena";
    private const string _apellidos = "Navarro Martínez";
    private const string _direccion = "Avda. España s/n, Albacete";
    private const string _telefono = "600123456";

    public CreateCompraAccesorios_test()
    {
        var cliente = new Cliente("1", _nombre, _apellidos, _userName, _direccion);

        var accesorios = new List<Accesorio>()
        {
            new Accesorio("Boquilla 0.4", CategoriaAccesorio.Boquillas, "Ender 3", 10, 5.0m),
            new Accesorio("Base magnética", CategoriaAccesorio.BasesImpresion, "Universal", 5, 20.0m),
        };

        _context.Add(cliente);
        _context.AddRange(accesorios);
        _context.SaveChanges();
    }

    public static IEnumerable<object[]> TestCasesFor_CreateCompraAccesorios_Error()
    {
        var sinLineas = new CompraAccesoriosForCreateDTO(_userName, _nombre, _apellidos, _direccion,
            _telefono, MetodoPago.tarjetaCredito, new List<LineaCompraAccesorioDTO>());

        var usuarioNoRegistrado = new CompraAccesoriosForCreateDTO("victor.lopez@uclm.es", _nombre, _apellidos,
            _direccion, _telefono, MetodoPago.tarjetaCredito,
            new List<LineaCompraAccesorioDTO>() { new LineaCompraAccesorioDTO(1, "Boquilla 0.4", 2, 5.0m) });

        var accesorioNoExiste = new CompraAccesoriosForCreateDTO(_userName, _nombre, _apellidos, _direccion,
            _telefono, MetodoPago.tarjetaCredito,
            new List<LineaCompraAccesorioDTO>() { new LineaCompraAccesorioDTO(99, "Inexistente", 1, 1.0m) });

        var sinStock = new CompraAccesoriosForCreateDTO(_userName, _nombre, _apellidos, _direccion,
            _telefono, MetodoPago.tarjetaCredito,
            new List<LineaCompraAccesorioDTO>() { new LineaCompraAccesorioDTO(1, "Boquilla 0.4", 11, 5.0m) });

        // primera línea válida y segunda sin stock suficiente
        var unaValidaOtraSinStock = new CompraAccesoriosForCreateDTO(_userName, _nombre, _apellidos, _direccion,
            _telefono, MetodoPago.tarjetaCredito,
            new List<LineaCompraAccesorioDTO>()
            {
                new LineaCompraAccesorioDTO(2, "Base magnética", 1, 20.0m),
                new LineaCompraAccesorioDTO(1, "Boquilla 0.4", 11, 5.0m),
            });

        return new List<object[]>
        {
            // compra a crear, comienzo del mensaje de error esperado
            new object[] { sinLineas, "Error! Debes incluir al menos un accesorio" },
            new object[] { usuarioNoRegistrado, "Error! El nombre de usuario" },
            new object[] { accesorioNoExiste, "Error! El accesorio con id 99 no existe" },
            new object[] { sinStock, "Error! No hay stock suficiente del accesorio 'Boquilla" },
            new object[] { unaValidaOtraSinStock, "Error! No hay stock suficiente del accesorio 'Boquilla" },
        };
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_CreateCompraAccesorios_Error))]
    public async Task CreateCompraAccesorios_Error_test(CompraAccesoriosForCreateDTO compraDTO, string errorExpected)
    {
        // Arrange
        var mock = new Mock<ILogger<CompraAccesoriosController>>();
        ILogger<CompraAccesoriosController> logger = mock.Object;
        var controller = new CompraAccesoriosController(_context, logger);

        // Act
        var result = await controller.CreateCompraAccesorios(compraDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        var errorActual = problemDetails.Errors.First().Value[0];
        Assert.StartsWith(errorExpected, errorActual);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateCompraAccesorios_Success_test()
    {
        // Arrange
        var mock = new Mock<ILogger<CompraAccesoriosController>>();
        ILogger<CompraAccesoriosController> logger = mock.Object;
        var controller = new CompraAccesoriosController(_context, logger);

        var compraDTO = new CompraAccesoriosForCreateDTO(_userName, _nombre, _apellidos, _direccion,
            _telefono, MetodoPago.paypal,
            new List<LineaCompraAccesorioDTO>()
            {
                new LineaCompraAccesorioDTO(1, "Boquilla 0.4", 2, 5.0m),
                new LineaCompraAccesorioDTO(2, "Base magnética", 1, 20.0m),
            });

        var expected = new CompraAccesoriosForGetDTO(1, DateTime.Now, _userName, _nombre, _apellidos,
            _direccion, _telefono, MetodoPago.paypal,
            new List<LineaCompraAccesorioDTO>()
            {
                new LineaCompraAccesorioDTO(1, "Boquilla 0.4", 2, 5.0m),
                new LineaCompraAccesorioDTO(2, "Base magnética", 1, 20.0m),
            });

        // Act
        var result = await controller.CreateCompraAccesorios(compraDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var actual = Assert.IsType<CompraAccesoriosForGetDTO>(createdResult.Value);
        Assert.Equal(expected, actual);

        // además, el stock se ha descontado en la base de datos
        using var contexto = CreateContext();
        Assert.Equal(8, contexto.Accesorios.Find(1)!.CantidadDisponible);
        Assert.Equal(4, contexto.Accesorios.Find(2)!.CantidadDisponible);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateCompraAccesorios_ErrorAlGuardar_test()
    {
        // Arrange: un contexto que falla al guardar, para cubrir el catch (409 Conflict)
        var mock = new Mock<ILogger<CompraAccesoriosController>>();
        ILogger<CompraAccesoriosController> logger = mock.Object;
        var contextoQueFalla = new ContextoQueFalla(_contextOptions);
        var controller = new CompraAccesoriosController(contextoQueFalla, logger);

        var compraDTO = new CompraAccesoriosForCreateDTO(_userName, _nombre, _apellidos, _direccion,
            _telefono, MetodoPago.tarjetaCredito,
            new List<LineaCompraAccesorioDTO>() { new LineaCompraAccesorioDTO(1, "Boquilla 0.4", 2, 5.0m) });

        // Act
        var result = await controller.CreateCompraAccesorios(compraDTO);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        var mensaje = Assert.IsType<string>(conflictResult.Value);
        Assert.StartsWith("Error", mensaje);
    }

    private class ContextoQueFalla : ApplicationDbContext
    {
        public ContextoQueFalla(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => throw new DbUpdateException("Error simulado al guardar");
    }
}
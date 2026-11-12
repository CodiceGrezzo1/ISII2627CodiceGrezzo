using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AppForSEII.API.Controllers;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.DTOs.Modelo3DDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.CompraModelo3DController_test;

public class CreateCompraModelo3D_test : AppForSEII4SqliteUT
{
    private const string _userName = "daniel@test.com";
    private const string _nombre = "Daniel";
    private const string _apellidos = "Gómez Blesa";
    private const string _direccion = "Calle Albacete 1";

    public CreateCompraModelo3D_test()
    {
        var cliente = new Cliente("1", _nombre, _apellidos, _userName, _direccion);

        var licencia1 = new LicenciaModelo3D("Licencia 1", new DateTime(2027, 11, 1));
        var licencia2 = new LicenciaModelo3D("Licencia 2", new DateTime(2027, 11, 1));

        var modelos = new List<Modelo3D>()
        {
            new Modelo3D("Dragon Articulado", "Miniaturas", FormatoModelo3D.STL, 15.0m, licencia1),
            new Modelo3D("Castillo", "Escenografia", FormatoModelo3D.STL, 20.0m, licencia2),
        };

        _context.Add(cliente);
        _context.AddRange(licencia1, licencia2);
        _context.AddRange(modelos);
        _context.SaveChanges();
    }

    // Simula un usuario autenticado: el controlador usa User.Identity.Name == Cliente.UserName
    private static void SetUser(CompraModelo3DController controller, string userName)
    {
        var identity = new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, userName) }, "TestAuth");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    public static IEnumerable<object[]> TestCasesFor_CreateCompraModelo3D_Error()
    {
        var compraValida = new CompraModelo3DForCreateDTO(_nombre, _apellidos, _userName, _direccion, null,
            MetodoPago.tarjetaCredito,
            new List<LineaCompraModeloForCreateDTO>() { new LineaCompraModeloForCreateDTO(2, 1) });

        var modeloNoExiste = new CompraModelo3DForCreateDTO(_nombre, _apellidos, _userName, _direccion, null,
            MetodoPago.tarjetaCredito,
            new List<LineaCompraModeloForCreateDTO>() { new LineaCompraModeloForCreateDTO(1, 99) });

        // primera línea válida y segunda con un modelo que no existe
        var unaValidaOtraNoExiste = new CompraModelo3DForCreateDTO(_nombre, _apellidos, _userName, _direccion, null,
            MetodoPago.tarjetaCredito,
            new List<LineaCompraModeloForCreateDTO>()
            {
                new LineaCompraModeloForCreateDTO(1, 1),
                new LineaCompraModeloForCreateDTO(1, 99),
            });
        var nombreIgualaCliente=new CompraModelo3DForCreateDTO("Cliente", _apellidos, _userName, _direccion, null,
            MetodoPago.tarjetaCredito,
            new List<LineaCompraModeloForCreateDTO>() { new LineaCompraModeloForCreateDTO(1, 99) });


        return new List<object[]>
        {
            // usuario autenticado, compra a crear, comienzo del mensaje de error esperado
            new object[] { "noexiste@test.com", compraValida, "Error! Client is not registered" },
            new object[] { _userName, modeloNoExiste, "Error! El modelo 3D indicado no existe" },
            new object[] { _userName, unaValidaOtraNoExiste, "Error! El modelo 3D indicado no existe" },
            new object[]{_userName,nombreIgualaCliente,"Error!Cliente no es un nombre valido"},
        };
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_CreateCompraModelo3D_Error))]
    public async Task CreateCompraModelo3D_Error_test(string userName, CompraModelo3DForCreateDTO compraDTO, string errorExpected)
    {
        // Arrange
        var mock = new Mock<ILogger<CompraModelo3DController>>();
        ILogger<CompraModelo3DController> logger = mock.Object;
        var controller = new CompraModelo3DController(_context, logger);
        SetUser(controller, userName);

        // Act
        var result = await controller.CreateCompraModelo3D(compraDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        var errorActual = problemDetails.Errors.First().Value[0];
        Assert.StartsWith(errorExpected, errorActual);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateCompraModelo3D_Success_test()
    {
        // Arrange
        var mock = new Mock<ILogger<CompraModelo3DController>>();
        ILogger<CompraModelo3DController> logger = mock.Object;
        var controller = new CompraModelo3DController(_context, logger);
        SetUser(controller, _userName);

        var compraDTO = new CompraModelo3DForCreateDTO(_nombre, _apellidos, _userName, _direccion, null,
            MetodoPago.paypal,
            new List<LineaCompraModeloForCreateDTO>()
            {
                new LineaCompraModeloForCreateDTO(2, 1), // 2 x 15.0 = 30.0
                new LineaCompraModeloForCreateDTO(1, 2), // 1 x 20.0 = 20.0
            });

        var expected = new CompraModelo3DForGetDto(1, DateTime.Now, _nombre, _apellidos, _userName,
            _direccion, null, 50.0m, "paypal");
        expected.LineasCompraModelo = new List<LineaCompraModeloForGetDto>()
        {
            new LineaCompraModeloForGetDto(1, 2, 15.0m, 30.0m,
                new Modelo3DForListDTO(1, "Dragon Articulado", "Miniaturas", "STL", 15.0m)),
            new LineaCompraModeloForGetDto(2, 1, 20.0m, 20.0m,
                new Modelo3DForListDTO(2, "Castillo", "Escenografia", "STL", 20.0m)),
        };

        // Act
        var result = await controller.CreateCompraModelo3D(compraDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CompraModelo3DController.GetCompraModelo3DDetails), createdResult.ActionName);

        var actual = Assert.IsType<CompraModelo3DForGetDto>(createdResult.Value);
        expected.FechaCompra = actual.FechaCompra; // el controlador pone DateTime.Now y no se puede predecir
        Assert.Equal(expected, actual);

        // además, la compra se ha guardado en la base de datos con sus dos líneas
        using var contexto = CreateContext();
        var guardada = contexto.ComprasModelo3D.Include(c => c.LineasCompraModelo).Single();
        Assert.Equal(50.0m, guardada.PrecioTotal);
        Assert.Equal(2, guardada.LineasCompraModelo.Count);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateCompraModelo3D_ErrorAlGuardar_test()
    {
        // Arrange: un contexto que falla al guardar, para cubrir el catch (409 Conflict)
        var mock = new Mock<ILogger<CompraModelo3DController>>();
        ILogger<CompraModelo3DController> logger = mock.Object;
        var contextoQueFalla = new ContextoQueFalla(_contextOptions);
        var controller = new CompraModelo3DController(contextoQueFalla, logger);
        SetUser(controller, _userName);

        var compraDTO = new CompraModelo3DForCreateDTO(_nombre, _apellidos, _userName, _direccion, null,
            MetodoPago.tarjetaCredito,
            new List<LineaCompraModeloForCreateDTO>() { new LineaCompraModeloForCreateDTO(2, 1) });

        // Act
        var result = await controller.CreateCompraModelo3D(compraDTO);

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
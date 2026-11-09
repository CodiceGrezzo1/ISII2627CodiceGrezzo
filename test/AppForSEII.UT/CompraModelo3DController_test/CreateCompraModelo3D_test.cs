using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AppForSEII.API.Controllers;
using AppForSEII.API.Data;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
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
        // Cada modelo con su propia licencia
        var licencia1 = new LicenciaModelo3D
        {
            Id = 1,
            Nombre = "Licencia 1",
            FechaExpiracion = new DateTime(2027, 11, 1)
        };
        var licencia2 = new LicenciaModelo3D
        {
            Id = 2,
            Nombre = "Licencia 2",
            FechaExpiracion = new DateTime(2027, 11, 1)
        };

        var modelos = new List<Modelo3D>()
        {
            new Modelo3D { Id = 1, Nombre = "Dragon Articulado", Categoria = "Minijaturas", Formato = FormatoModelo3D.STL, Precio = 15.0m, LicenciaModelo3D = licencia1 },
            new Modelo3D { Id = 2, Nombre = "Castillo", Categoria = "Escenografia", Formato = FormatoModelo3D.STL, Precio = 20.0m, LicenciaModelo3D = licencia2 },
        };

        var cliente = new Cliente
        {
            Id = "cliente-1",
            UserName = _userName,
            Email = _userName
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

        return new List<object[]>
        {
            // usuario autenticado, compra a crear, comienzo del mensaje de error esperado
            new object[] { "noexiste@test.com", compraValida, "Error! Client is not registered" },
            new object[] { _userName, modeloNoExiste, "Error! El modelo 3D indicado no existe" },
            new object[] { _userName, unaValidaOtraNoExiste, "Error! El modelo 3D indicado no existe" },
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

        // Act
        var result = await controller.CreateCompraModelo3D(compraDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CompraModelo3DController.GetCompraModelo3DDetails), createdResult.ActionName);

        var actual = Assert.IsType<CompraModelo3DForGetDto>(createdResult.Value);
        Assert.Equal(1, actual.Id);
        Assert.Equal(_nombre, actual.NombreCliente);
        Assert.Equal(_apellidos, actual.ApellidosCliente);
        Assert.Equal("paypal", actual.MetodoPago);
        Assert.Equal(50.0m, actual.PrecioTotal);
        Assert.Equal(2, actual.LineasCompraModelo.Count);

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
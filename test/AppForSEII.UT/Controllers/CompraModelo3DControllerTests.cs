using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppForSEII.API.Controllers;
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.CompraModelo3DDTOs;
using AppForSEII.API.DTOs.Modelo3DDTOs;

namespace AppForSEII.UT.Controllers;

public class CompraModelo3DControllerTests
{
    private ApplicationDbContext GetSqliteDbContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        // Sembramos datos iniciales de prueba (Arrange base)
        var licencia = new LicenciaModelo3D
        {
            Id = 1,
            Nombre = "Licencia de prueba",
            FechaExpiracion = DateTime.Today.AddYears(1)
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
            FechaCompra = DateTime.Now,
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

        context.Users.Add(cliente);
        context.LicenciasModelo3D.Add(licencia);
        context.Modelos3D.Add(modelo);
        context.ComprasModelo3D.Add(compra);
        context.SaveChanges();

        return context;
    }

    // Simula un usuario autenticado: el controlador usa User.Identity.Name == Cliente.UserName
    private static void SetUser(CompraModelo3DController controller, string userName)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userName)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }

    // ==========================================
    // GET (Listado)
    // ==========================================
    [Fact]
    public async Task GetComprasModelo3D_ReturnsOkResult_WithList()
    {
        // Arrange
        using var context = GetSqliteDbContext();
        var mockLogger = new Mock<ILogger<CompraModelo3DController>>();
        var controller = new CompraModelo3DController(context, mockLogger.Object);

        // Act
        var result = await controller.GetComprasModelo3D(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var compras = Assert.IsAssignableFrom<IList<CompraModelo3DForGetDto>>(okResult.Value);
        Assert.Single(compras);
    }

    // ==========================================
    // SELECT (Detalles)
    // ==========================================
    [Fact]
    public async Task GetCompraModelo3DDetails_ReturnsNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        using var context = GetSqliteDbContext();
        var mockLogger = new Mock<ILogger<CompraModelo3DController>>();
        var controller = new CompraModelo3DController(context, mockLogger.Object);

        // Act
        var result = await controller.GetCompraModelo3DDetails(999); // ID inexistente

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetCompraModelo3DDetails_ReturnsOk_WhenIdExists()
    {
        // Arrange
        using var context = GetSqliteDbContext();
        var mockLogger = new Mock<ILogger<CompraModelo3DController>>();
        var controller = new CompraModelo3DController(context, mockLogger.Object);

        // Act
        var result = await controller.GetCompraModelo3DDetails(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var compraDto = Assert.IsType<CompraModelo3DForGetDto>(okResult.Value);
        Assert.Equal(1, compraDto.Id);
    }

    // ==========================================
    // POST (Creación)
    // ==========================================
    [Fact]
    public async Task CreateCompraModelo3D_ReturnsCreatedAtAction_WhenDataIsValid()
    {
        // Arrange
        using var context = GetSqliteDbContext();
        var mockLogger = new Mock<ILogger<CompraModelo3DController>>();
        var controller = new CompraModelo3DController(context, mockLogger.Object);
        SetUser(controller, "daniel@test.com"); // coincide con el UserName del Cliente sembrado

        var dto = new CompraModelo3DForCreateDTO
        {
            NombreCliente = "Daniel",
            ApellidosCliente = "Gómez Blesa",
            CorreoElectronico = "daniel@test.com",
            DireccionFacturacion = "Calle Albacete 1",
            MetodoPago = MetodoPago.tarjetaCredito,
            LineasCompraModelo = new List<LineaCompraModeloForCreateDTO>
            {
                new LineaCompraModeloForCreateDTO(2, 1) // cantidadLicencias: 2, modelo3DId: 1
            }
        };

        //ACT
        var result = await controller.CreateCompraModelo3D(dto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<CompraModelo3DForGetDto>(createdAtActionResult.Value);
        Assert.Equal("Daniel", returnValue.NombreCliente);
        Assert.Equal(30.0m, returnValue.PrecioTotal); // 2 licencias x 15.0
    }
}   
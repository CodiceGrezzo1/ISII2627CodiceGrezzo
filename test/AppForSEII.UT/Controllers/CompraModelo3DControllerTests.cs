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
    public async Task PostCompraModelo3D_ReturnsCreatedAtAction_WhenDataIsValid()
    {
        // Arrange
        using var context = GetSqliteDbContext();
        var mockLogger = new Mock<ILogger<CompraModelo3DController>>();
        var controller = new CompraModelo3DController(context, mockLogger.Object);
        controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(
                    new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "daniel@test.com") },
                    "Test"))
            }
        };

        var dto = new CompraModelo3DForCreateDTO
        {
            NombreCliente = "Daniel",
            ApellidosCliente = "Gómez Blesa",
            CorreoElectronico = "daniel@test.com",
            DireccionFacturacion = "Calle Albacete 1",
            MetodoPago = MetodoPago.tarjetaCredito,
            LineasCompraModelo = new List<LineaCompraModeloForCreateDTO>
            {
                new LineaCompraModeloForCreateDTO(2, 1)
            }
        };

        // Act
        var result = await controller.CreateCompraModelo3D(dto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<CompraModelo3DForGetDto>(createdAtActionResult.Value);
        Assert.Equal("Daniel", returnValue.NombreCliente);
    }
}
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ApplicationUserDTOs.Accesorio3DDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.Accesorio3DController_test;

public class GetAccesorios3D_test : AppForSEII4SqliteUT
{
    public GetAccesorios3D_test()
    {
        var accesorios = new List<Accesorio>()
        {
            new Accesorio("Boquilla 0.4", CategoriaAccesorio.Boquillas, "Ender 3", 10, 5.0m),
            new Accesorio("Base magnética", CategoriaAccesorio.BasesImpresion, "Universal", 5, 20.0m),
            // sin stock: no debe aparecer nunca (flujo alternativo 2.1)
            new Accesorio("Boquilla agotada", CategoriaAccesorio.Boquillas, "Ender 3", 0, 4.0m),
        };

        _context.AddRange(accesorios);
        _context.SaveChanges();
    }

    public static IEnumerable<object?[]> TestCasesFor_GetAccesorios3D_OK()
    {
        var boquilla = new Accesorio3DForGetDTO(1, "Boquilla 0.4", CategoriaAccesorio.Boquillas, "Ender 3", 10, 5.0m);
        var baseMag = new Accesorio3DForGetDTO(2, "Base magnética", CategoriaAccesorio.BasesImpresion, "Universal", 5, 20.0m);

        return new List<object?[]>
        {
            //nombre, categoria, accesorios esperados (ordenados por nombre)
            new object?[] { null, null, new List<Accesorio3DForGetDTO>() { baseMag, boquilla } },        // caso 1
            new object?[] { null, "Boquillas", new List<Accesorio3DForGetDTO>() { boquilla } },          // caso 2
            new object?[] { "Boq", null, new List<Accesorio3DForGetDTO>() { boquilla } },                // caso 3
            new object?[] { "Boq", "Boquillas", new List<Accesorio3DForGetDTO>() { boquilla } },         // caso 4
        };
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_GetAccesorios3D_OK))]
    public async Task GetAccesorios3D_OK_test(string? nombre, string? categoria, List<Accesorio3DForGetDTO> expected)
    {
        // Arrange
        var mock = new Mock<ILogger<Accesorio3DController>>();
        ILogger<Accesorio3DController> logger = mock.Object;
        var controller = new Accesorio3DController(_context, logger);

        // Act
        var result = await controller.GetAccesorios3D(nombre, categoria);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<List<Accesorio3DForGetDTO>>(okResult.Value);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [InlineData("xyz")] // caso 5: no se puede convertir al enum
    [InlineData("99")]  // caso 6: se convierte, pero no es un valor definido
    public async Task GetAccesorios3D_CategoriaInvalida_test(string categoria)
    {
        // Arrange
        var mock = new Mock<ILogger<Accesorio3DController>>();
        ILogger<Accesorio3DController> logger = mock.Object;
        var controller = new Accesorio3DController(_context, logger);

        // Act
        var result = await controller.GetAccesorios3D(null, categoria);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var mensaje = Assert.IsType<string>(badRequest.Value);
        Assert.StartsWith("Categoría no válida", mensaje);
    }
}
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Pieza3DDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.Controllers;

public class GetPiezas3D_test : AppForSEII4SqliteUT {
    public GetPiezas3D_test() {
        var piezas = new List<Pieza3D>() {
            new Pieza3D("Figura dragón", 0.5m, CategoriaPieza.Decoracion),
            new Pieza3D("Soldadito", 0.2m, CategoriaPieza.MiniaturasYMaquetas),
            new Pieza3D("Tornillo repuesto", 0.1m, CategoriaPieza.Repuestos),
            new Pieza3D("Alicates", 1.2m, CategoriaPieza.HerramientasYAccesorios),
        };

        _context.AddRange(piezas);
        _context.SaveChanges();
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetPiezas3D_NULL_NAME_CATEGORIA() {
        // Arrange
        var expectedPiezas = new List<Pieza3DForListDTO>() {
            new Pieza3DForListDTO(1, "Figura dragón", 0.5m, CategoriaPieza.Decoracion.ToString()),
            new Pieza3DForListDTO(2, "Soldadito", 0.2m, CategoriaPieza.MiniaturasYMaquetas.ToString()),
            new Pieza3DForListDTO(3, "Tornillo repuesto", 0.1m, CategoriaPieza.Repuestos.ToString()),
            new Pieza3DForListDTO(4, "Alicates", 1.2m, CategoriaPieza.HerramientasYAccesorios.ToString()),
        };
        var mock = new Mock<ILogger<Piezas3DController>>();
        ILogger<Piezas3DController> logger = mock.Object;

        var controller = new Piezas3DController(_context, logger);

        // Act
        var result = await controller.GetPiezas3D(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var piezasDTOsActual = Assert.IsType<List<Pieza3DForListDTO>>(okResult.Value);
        Assert.Equal(expectedPiezas, piezasDTOsActual);
    }

    public static IEnumerable<object?[]> TestCasesFor_GetPiezas3D_OK() {
        var piezaDTOs = new List<Pieza3DForListDTO>() {
            new Pieza3DForListDTO(1, "Figura dragón", 0.5m, CategoriaPieza.Decoracion.ToString()),
            new Pieza3DForListDTO(2, "Soldadito", 0.2m, CategoriaPieza.MiniaturasYMaquetas.ToString()),
            new Pieza3DForListDTO(3, "Tornillo repuesto", 0.1m, CategoriaPieza.Repuestos.ToString()),
            new Pieza3DForListDTO(4, "Alicates", 1.2m, CategoriaPieza.HerramientasYAccesorios.ToString()),
        };

        // TC1: Sin filtros (ordenadas por categoría y nombre)
        var piezaDTOsTC1 = new List<Pieza3DForListDTO>() { piezaDTOs[0], piezaDTOs[1], piezaDTOs[2], piezaDTOs[3] };
        // TC2: Filtro por nombre "Tornillo"
        var piezaDTOsTC2 = new List<Pieza3DForListDTO>() { piezaDTOs[2] }; 
        // TC3: Filtro por categoría HerramientasYAccesorios
        var piezaDTOsTC3 = new List<Pieza3DForListDTO>() { piezaDTOs[3] }; 

        var allTests = new List<object?[]>
        {
            new object?[] { null, (CategoriaPieza?)null, piezaDTOsTC1 },
            new object?[] { "Tornillo", (CategoriaPieza?)null, piezaDTOsTC2 },
            new object?[] { null, (CategoriaPieza?)CategoriaPieza.HerramientasYAccesorios, piezaDTOsTC3 },
        };

        return allTests;
    }

    [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_GetPiezas3D_OK))]
    public async Task GetPiezas3D_filter_test(string? name, CategoriaPieza? categoria, List<Pieza3DForListDTO> expectedPiezas) {
        // Arrange
        var mock = new Mock<ILogger<Piezas3DController>>();
        ILogger<Piezas3DController> logger = mock.Object;
        var controller = new Piezas3DController(_context, logger);

        // Act
        var result = await controller.GetPiezas3D(name, categoria);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var piezasDTOsActual = Assert.IsType<List<Pieza3DForListDTO>>(okResult.Value);
        Assert.Equal(expectedPiezas, piezasDTOsActual);
    }
}
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.Impresora3DDTOs;

namespace AppForSEII.UT.Impresoras3DController_test;

// Complejidad ciclomática de GetImpresorasParaReservar = 6 (grafo de 9 nodos y 13 aristas):
// 5 nodos predicado (nombre == null, Nombre.Contains, tipo == null, Tipo == tipo, Count == 0) + 1.
// Caminos independientes y su prueba:
//   C1 1-3-5-6-7-9    SinFiltros
//   C2 1-2-3-5-6-7-9  FiltroNombre
//   C3 1-3-4-5-6-7-9  FiltroTipo
//   C4 1-2-6-7-8-9    SinResultados
//   C5 1-3-4-6-7-8-9  TipoSinImpresoras
//   C6 1-3-4-6-7-9    FiltroTipoFilamento
// FiltroNombreYTipo es una prueba extra (1-2-3-4-5-6-7-9).
public class GetImpresorasParaReservar_test : AppForSEII4SqliteUT
{
    private readonly ImpresoraParaReservarDTO _anycubic =
        new ImpresoraParaReservarDTO(2, "Anycubic Photon", "Mono X", TipoImpresora.Resina, "Impresora de resina", 0.20m, 4.00m);
    private readonly ImpresoraParaReservarDTO _ender3 =
        new ImpresoraParaReservarDTO(1, "Ender 3", "V2", TipoImpresora.Filamento, "Impresora de filamento", 0.15m, 2.50m);
    private readonly ImpresoraParaReservarDTO _ender5 =
        new ImpresoraParaReservarDTO(3, "Ender 5", "Plus", TipoImpresora.Filamento, null, 0.18m, 3.00m);

    public GetImpresorasParaReservar_test()
    {
        // Datos de partida: tres impresoras (dos de filamento y una de resina)
        _context.Impresoras3D.AddRange(
            new Impresora3D { Id = 1, Nombre = "Ender 3", Modelo = "V2", Tipo = TipoImpresora.Filamento, Descripcion = "Impresora de filamento", PrecioKilovatioHora = 0.15m, PrecioReserva = 2.50m },
            new Impresora3D { Id = 2, Nombre = "Anycubic Photon", Modelo = "Mono X", Tipo = TipoImpresora.Resina, Descripcion = "Impresora de resina", PrecioKilovatioHora = 0.20m, PrecioReserva = 4.00m },
            new Impresora3D { Id = 3, Nombre = "Ender 5", Modelo = "Plus", Tipo = TipoImpresora.Filamento, Descripcion = null, PrecioKilovatioHora = 0.18m, PrecioReserva = 3.00m });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetImpresorasParaReservar_SinFiltros_test()
    {
        // Arrange
        var mock = new Mock<ILogger<Impresoras3DController>>();
        var controller = new Impresoras3DController(_context, mock.Object);
        // Resultado esperado: todas, ordenadas por nombre
        var esperado = new List<ImpresoraParaReservarDTO> { _anycubic, _ender3, _ender5 };

        // Act
        var result = await controller.GetImpresorasParaReservar(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var impresoras = Assert.IsAssignableFrom<IList<ImpresoraParaReservarDTO>>(okResult.Value);
        Assert.Equal(esperado, impresoras);
    }

    [Fact]
    public async Task GetImpresorasParaReservar_FiltroNombre_test()
    {
        // Arrange
        var mock = new Mock<ILogger<Impresoras3DController>>();
        var controller = new Impresoras3DController(_context, mock.Object);
        var esperado = new List<ImpresoraParaReservarDTO> { _ender3, _ender5 };

        // Act
        var result = await controller.GetImpresorasParaReservar("Ender", null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var impresoras = Assert.IsAssignableFrom<IList<ImpresoraParaReservarDTO>>(okResult.Value);
        Assert.Equal(esperado, impresoras);
    }

    [Fact]
    public async Task GetImpresorasParaReservar_FiltroTipo_test()
    {
        // Arrange
        var mock = new Mock<ILogger<Impresoras3DController>>();
        var controller = new Impresoras3DController(_context, mock.Object);
        var esperado = new List<ImpresoraParaReservarDTO> { _anycubic };

        // Act
        var result = await controller.GetImpresorasParaReservar(null, TipoImpresora.Resina);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var impresoras = Assert.IsAssignableFrom<IList<ImpresoraParaReservarDTO>>(okResult.Value);
        Assert.Equal(esperado, impresoras);
    }

    [Fact]
    public async Task GetImpresorasParaReservar_FiltroNombreYTipo_test()
    {
        // Arrange
        var mock = new Mock<ILogger<Impresoras3DController>>();
        var controller = new Impresoras3DController(_context, mock.Object);
        var esperado = new List<ImpresoraParaReservarDTO> { _ender5 };

        // Act
        var result = await controller.GetImpresorasParaReservar("Ender 5", TipoImpresora.Filamento);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var impresoras = Assert.IsAssignableFrom<IList<ImpresoraParaReservarDTO>>(okResult.Value);
        Assert.Equal(esperado, impresoras);
    }

    [Fact]
    public async Task GetImpresorasParaReservar_SinResultados_test()
    {
        // Arrange
        var mock = new Mock<ILogger<Impresoras3DController>>();
        var controller = new Impresoras3DController(_context, mock.Object);

        // Act
        var result = await controller.GetImpresorasParaReservar("Prusa", null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var impresoras = Assert.IsAssignableFrom<IList<ImpresoraParaReservarDTO>>(okResult.Value);
        Assert.Empty(impresoras);
    }

    [Fact]
    public async Task GetImpresorasParaReservar_TipoSinImpresoras_test()
    {
        // Arrange
        var mock = new Mock<ILogger<Impresoras3DController>>();
        var controller = new Impresoras3DController(_context, mock.Object);

        // Act
        var result = await controller.GetImpresorasParaReservar(null, TipoImpresora.Polimero);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var impresoras = Assert.IsAssignableFrom<IList<ImpresoraParaReservarDTO>>(okResult.Value);
        Assert.Empty(impresoras);
    }

    [Fact]
    public async Task GetImpresorasParaReservar_FiltroTipoFilamento_test()
    {
        // Arrange
        var mock = new Mock<ILogger<Impresoras3DController>>();
        var controller = new Impresoras3DController(_context, mock.Object);
        var esperado = new List<ImpresoraParaReservarDTO> { _ender3, _ender5 };

        // Act
        var result = await controller.GetImpresorasParaReservar(null, TipoImpresora.Filamento);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var impresoras = Assert.IsAssignableFrom<IList<ImpresoraParaReservarDTO>>(okResult.Value);
        Assert.Equal(esperado, impresoras);
    }
}
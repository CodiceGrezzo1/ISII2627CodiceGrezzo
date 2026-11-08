using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.EncargoImpresionDTOs;
using AppForSEII.API.DTOs.Pieza3DDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII.UT.EncargosImpresionController_test
{
    public class CreateEncargosImpresion_test : AppForSEII4SqliteUT
    {
        private const string _userName = "juan@uclm.es";
        private const string _customerName = "Juan";
        private const string _customerSurname = "Pérez Gómez";
        private const string _deliveryAddress = "Calle Ancha 1, Albacete";

        private const string _pieza1Nombre = "Figura dragón";
        private const string _pieza2Nombre = "Soporte móvil";

        public CreateEncargosImpresion_test()
        {
            var cliente = new Cliente("1", _customerName, _customerSurname, _userName, _deliveryAddress);

            var piezas = new List<Pieza3D>() {
                new Pieza3D(_pieza1Nombre, 0.5m, CategoriaPieza.Decoracion),
                new Pieza3D(_pieza2Nombre, 0.2m, CategoriaPieza.HerramientasYAccesorios),
            };

            var material = new Material("PLA", 0.05m, 1000m);

            _context.Users.Add(cliente);
            _context.AddRange(piezas);
            _context.Add(material);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateEncargo()
        {
            var lineaDTOs = new List<LineaEncargoForCreateDTO>() {
                new LineaEncargoForCreateDTO(2, 1, 1) // cantidad, pieza3dId, materialId
            };

            var encargoInvalidUser = new EncargoImpresionForCreateDTO(
                _customerName, _customerSurname, _deliveryAddress, "600123456", "Prueba", MetodoPago.tarjetaCredito, lineaDTOs
            );

            var allTests = new List<object[]>
            {
                new object[] { encargoInvalidUser, "Error! Client is not registered or authenticated." },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateEncargo))]
        public async Task CreateEncargoImpresion_Error_test(EncargoImpresionForCreateDTO createDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            ILogger<EncargosImpresionController> logger = mock.Object;

            var controller = new EncargosImpresionController(_context, logger);

            // Simulamos un usuario autenticado pero que NO está registrado en la BD para forzar el error
            var userMock = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "noexiste@uclm.es")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() { User = userMock }
            };

            // Act
            var result = await controller.CreateEncargoImpresion(createDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateEncargoImpresion_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            ILogger<EncargosImpresionController> logger = mock.Object;

            var controller = new EncargosImpresionController(_context, logger);

            // Simulamos el usuario autenticado correcto en el contexto HTTP del controlador
            var userMock = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, _userName)
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() { User = userMock }
            };

            var createDTO = new EncargoImpresionForCreateDTO(
                _customerName,
                _customerSurname,
                _deliveryAddress,
                "600123456",
                "Encargo de prueba exitoso",
                MetodoPago.tarjetaCredito,
                new List<LineaEncargoForCreateDTO>()
                {
                    new LineaEncargoForCreateDTO(2, 1, 1) // Cantidad 2, Pieza 1, Material 1
                }
            );

            // Act
            var result = await controller.CreateEncargoImpresion(createDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualEncargo = Assert.IsType<EncargoImpresion>(okResult.Value);

            Assert.NotNull(actualEncargo);
            Assert.Equal(_customerName, actualEncargo.NombreCliente);
            Assert.Single(actualEncargo.LineasEncargo);
            Assert.Equal(0.10m, actualEncargo.PrecioTotal); // 2 unidades * 0.05 precio por gramo del material
        }
    }
}
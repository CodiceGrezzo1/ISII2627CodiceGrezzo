using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.EncargoImpresionDTOs;
using AppForSEII.API.DTOs.Pieza3DDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppForSEII.UT.EncargosImpresionController_test
{
    public class GetEncargoImpresionDetails_test : AppForSEII4SqliteUT
    {
        public GetEncargoImpresionDetails_test()
        {
            // 1. Crear los datos necesarios en el contexto de SQLite en memoria
            var cliente = new Cliente("1", "Juan", "Pérez Gómez", "juan@uclm.es", "Calle Ancha 1, Albacete");

            var piezas = new List<Pieza3D>() {
                new Pieza3D("Figura dragón", 0.5m, CategoriaPieza.Decoracion),
            };

            var material = new Material("PLA", 0.05m, 1000m);

            var encargo = new EncargoImpresion
            {
                FechaEncargo = new DateTime(2026, 5, 10),
                NombreCliente = "Juan",
                ApellidosCliente = "Pérez Gómez",
                DireccionEnvio = "Calle Ancha 1, Albacete",
                NumeroTelefono = "600123456",
                Descripcion = "Impresión de prueba",
                MetodoPago = MetodoPago.bizum, // Ajusta el enum según tu modelo si difiere
                PrecioTotal = 2.50m,
                Cliente = cliente,
                LineasEncargo = new List<LineaEncargo>()
            };

            var linea = new LineaEncargo
            {
                Cantidad = 5,
                PrecioUnidad = 0.50m,
                Subtotal = 2.50m,
                Pieza3D = piezas[0],
                Material = material,
                EncargoImpresion = encargo
            };

            encargo.LineasEncargo.Add(linea);

            // 2. Añadir los elementos al contexto
            _context.Users.Add(cliente);
            _context.AddRange(piezas);
            _context.Add(material);
            _context.Add(encargo);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetEncargoImpresionDetails_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            ILogger<EncargosImpresionController> logger = mock.Object;

            var controller = new EncargosImpresionController(_context, logger);

            // Act
            var result = await controller.GetEncargoImpresionDetails(999); // ID que no existe

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetEncargoImpresionDetails_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<EncargosImpresionController>>();
            ILogger<EncargosImpresionController> logger = mock.Object;
            var controller = new EncargosImpresionController(_context, logger);

            // Preparar el DTO esperado con sus líneas y detalles
            var expectedEncargo = new EncargoImpresionForGetDTO(
                1,
                new DateTime(2026, 5, 10),
                "Juan",
                "Pérez Gómez",
                "Calle Ancha 1, Albacete",
                "600123456",
                "Impresión de prueba",
                2.50m,
                MetodoPago.bizum.ToString()
            );

            expectedEncargo.LineasEncargo = new List<LineaEncargoForGetDTO>
            {
                new LineaEncargoForGetDTO(
                    1,
                    5,
                    0.50m,
                    2.50m,
                    new Pieza3DForListDTO(1, "Figura dragón", 0.5m, CategoriaPieza.Decoracion.ToString())
                )
            };

            // Act 
            var result = await controller.GetEncargoImpresionDetails(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var encargoDTOActual = Assert.IsType<EncargoImpresionForGetDTO>(okResult.Value);
            
            // Comprobar que el DTO obtenido coincide exactamente con el esperado
            Assert.Equal(expectedEncargo, encargoDTOActual);
        }
    }
}
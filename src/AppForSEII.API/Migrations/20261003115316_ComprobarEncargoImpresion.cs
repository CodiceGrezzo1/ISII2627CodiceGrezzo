using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII.API.Migrations
{
    /// <inheritdoc />
    public partial class ComprobarEncargoImpresion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EncargoImpresiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaEncargo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreCliente = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApellidosCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NumeroTelefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrecioTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncargoImpresiones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncargoImpresiones");
        }
    }
}

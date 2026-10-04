using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReservasImpresoras3D : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DireccionFacturacion",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Impresoras3D",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrecioKilovatioHora = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    PrecioReserva = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impresoras3D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservasImpresora",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaReserva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidosCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DireccionFacturacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PrecioTotal = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasImpresora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasImpresora_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineasReserva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TiempoReserva = table.Column<int>(type: "int", nullable: false),
                    PrecioSubtotal = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ReservaImpresoraId = table.Column<int>(type: "int", nullable: false),
                    Impresora3DId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasReserva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasReserva_Impresoras3D_Impresora3DId",
                        column: x => x.Impresora3DId,
                        principalTable: "Impresoras3D",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineasReserva_ReservasImpresora_ReservaImpresoraId",
                        column: x => x.ReservaImpresoraId,
                        principalTable: "ReservasImpresora",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineasReserva_Impresora3DId",
                table: "LineasReserva",
                column: "Impresora3DId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasReserva_ReservaImpresoraId",
                table: "LineasReserva",
                column: "ReservaImpresoraId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasImpresora_ClienteId",
                table: "ReservasImpresora",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineasReserva");

            migrationBuilder.DropTable(
                name: "Impresoras3D");

            migrationBuilder.DropTable(
                name: "ReservasImpresora");

            migrationBuilder.DropColumn(
                name: "DireccionFacturacion",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}

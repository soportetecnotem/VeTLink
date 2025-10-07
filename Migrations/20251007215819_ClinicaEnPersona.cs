using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeTLink.Migrations
{
    /// <inheritdoc />
    public partial class ClinicaEnPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sucursales_Veterinarios_VeterinarioId",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Sucursales_VeterinarioId",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "VeterinarioId",
                table: "Sucursales");

            migrationBuilder.AddColumn<int>(
                name: "ClinicaId",
                table: "Personas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SucursalVeterinario",
                columns: table => new
                {
                    SucursalesAsignadasId = table.Column<int>(type: "int", nullable: false),
                    VeterinariosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SucursalVeterinario", x => new { x.SucursalesAsignadasId, x.VeterinariosId });
                    table.ForeignKey(
                        name: "FK_SucursalVeterinario_Sucursales_SucursalesAsignadasId",
                        column: x => x.SucursalesAsignadasId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SucursalVeterinario_Veterinarios_VeterinariosId",
                        column: x => x.VeterinariosId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personas_ClinicaId",
                table: "Personas",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_SucursalVeterinario_VeterinariosId",
                table: "SucursalVeterinario",
                column: "VeterinariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Clinicas_ClinicaId",
                table: "Personas",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Clinicas_ClinicaId",
                table: "Personas");

            migrationBuilder.DropTable(
                name: "SucursalVeterinario");

            migrationBuilder.DropIndex(
                name: "IX_Personas_ClinicaId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "ClinicaId",
                table: "Personas");

            migrationBuilder.AddColumn<Guid>(
                name: "VeterinarioId",
                table: "Sucursales",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_VeterinarioId",
                table: "Sucursales",
                column: "VeterinarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sucursales_Veterinarios_VeterinarioId",
                table: "Sucursales",
                column: "VeterinarioId",
                principalTable: "Veterinarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

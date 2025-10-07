using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeTLink.Migrations
{
    /// <inheritdoc />
    public partial class Sucursales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clinicas_Direcciones_DireccionId",
                table: "Clinicas");

            migrationBuilder.DropForeignKey(
                name: "FK_Clinicas_Veterinarios_VeterinarioId",
                table: "Clinicas");

            migrationBuilder.DropIndex(
                name: "IX_Clinicas_DireccionId",
                table: "Clinicas");

            migrationBuilder.DropIndex(
                name: "IX_Clinicas_VeterinarioId",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "DireccionId",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "VeterinarioId",
                table: "Clinicas");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Clinicas",
                newName: "Logo");

            migrationBuilder.CreateTable(
                name: "Sucursales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreSucursal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoSucursal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Horario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    DireccionId = table.Column<int>(type: "int", nullable: true),
                    ClinicaId = table.Column<int>(type: "int", nullable: true),
                    VeterinarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sucursales_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sucursales_Direcciones_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "Direcciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sucursales_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_ClinicaId",
                table: "Sucursales",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_DireccionId",
                table: "Sucursales",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_VeterinarioId",
                table: "Sucursales",
                column: "VeterinarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sucursales");

            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "Clinicas",
                newName: "Telefono");

            migrationBuilder.AddColumn<int>(
                name: "DireccionId",
                table: "Clinicas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clinicas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VeterinarioId",
                table: "Clinicas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_DireccionId",
                table: "Clinicas",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_VeterinarioId",
                table: "Clinicas",
                column: "VeterinarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clinicas_Direcciones_DireccionId",
                table: "Clinicas",
                column: "DireccionId",
                principalTable: "Direcciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clinicas_Veterinarios_VeterinarioId",
                table: "Clinicas",
                column: "VeterinarioId",
                principalTable: "Veterinarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

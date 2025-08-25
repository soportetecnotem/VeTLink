using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeTLink.Migrations
{
    /// <inheritdoc />
    public partial class UserPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagen",
                table: "Personas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Personas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "HistorialMedicoId",
                table: "Alergias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_UsuarioId",
                table: "Personas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Alergias_HistorialMedicoId",
                table: "Alergias",
                column: "HistorialMedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alergias_HistorialesMedicos_HistorialMedicoId",
                table: "Alergias",
                column: "HistorialMedicoId",
                principalTable: "HistorialesMedicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_AspNetUsers_UsuarioId",
                table: "Personas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alergias_HistorialesMedicos_HistorialMedicoId",
                table: "Alergias");

            migrationBuilder.DropForeignKey(
                name: "FK_Personas_AspNetUsers_UsuarioId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_UsuarioId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Alergias_HistorialMedicoId",
                table: "Alergias");

            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "HistorialMedicoId",
                table: "Alergias");
        }
    }
}

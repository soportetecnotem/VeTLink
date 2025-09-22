using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeTLink.Migrations
{
    /// <inheritdoc />
    public partial class AddSuscripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clinicas_Planes_PlanId",
                table: "Clinicas");

            migrationBuilder.DropIndex(
                name: "IX_Clinicas_PlanId",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "Vigencia",
                table: "Planes");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "Clinicas",
                newName: "SuscripcionId");

            migrationBuilder.CreateTable(
                name: "EstadosSuscripcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosSuscripcion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suscripciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Renovacion = table.Column<bool>(type: "bit", nullable: false),
                    Vigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoSuscripcionId = table.Column<int>(type: "int", nullable: true),
                    PlanId = table.Column<int>(type: "int", nullable: true),
                    ClinicaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suscripciones_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suscripciones_EstadosSuscripcion_EstadoSuscripcionId",
                        column: x => x.EstadoSuscripcionId,
                        principalTable: "EstadosSuscripcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suscripciones_Planes_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Planes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_ClinicaId",
                table: "Suscripciones",
                column: "ClinicaId",
                unique: true,
                filter: "[ClinicaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_EstadoSuscripcionId",
                table: "Suscripciones",
                column: "EstadoSuscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_PlanId",
                table: "Suscripciones",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suscripciones");

            migrationBuilder.DropTable(
                name: "EstadosSuscripcion");

            migrationBuilder.RenameColumn(
                name: "SuscripcionId",
                table: "Clinicas",
                newName: "PlanId");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Planes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Vigencia",
                table: "Planes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_PlanId",
                table: "Clinicas",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clinicas_Planes_PlanId",
                table: "Clinicas",
                column: "PlanId",
                principalTable: "Planes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

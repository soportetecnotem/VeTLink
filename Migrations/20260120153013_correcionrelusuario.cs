using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeTLink.Migrations
{
    /// <inheritdoc />
    public partial class correcionrelusuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LlavesAPI_AspNetUsers_UsuarioId",
                table: "LlavesAPI");

            migrationBuilder.DropIndex(
                name: "IX_LlavesAPI_UsuarioId",
                table: "LlavesAPI");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "LlavesAPI",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "TipoLlave",
                table: "LlavesAPI",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoLlave",
                table: "LlavesAPI");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "LlavesAPI",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LlavesAPI_UsuarioId",
                table: "LlavesAPI",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_LlavesAPI_AspNetUsers_UsuarioId",
                table: "LlavesAPI",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

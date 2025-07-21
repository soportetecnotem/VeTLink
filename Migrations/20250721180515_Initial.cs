using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeTLink.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alergias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sustancia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carnet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carnet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comportamientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comportamientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CondicionesCorporal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionesCorporal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diagnosticos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiagnosticoPresuntivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagnosticoDefinitivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosticos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Direcciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Calle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoInt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoExt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Colonia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Municipio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direcciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enfermedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enfermedades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosGeneral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosGeneral", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hidrataciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hidrataciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesReproductivos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Esterilizado = table.Column<bool>(type: "bit", nullable: true),
                    FechaEsterilizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Partos = table.Column<int>(type: "int", nullable: false),
                    Montas = table.Column<int>(type: "int", nullable: false),
                    UltimoCelo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrimeraGesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimaGesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesReproductivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mucosas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mucosas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NivelActividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelActividades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReaccionesSociales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReaccionesSociales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposCirugias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCirugias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposPruebas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prueba = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPruebas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Servicio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposServicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposTratamientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTratamientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposUsuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesTiempo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesTiempo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViasAdministracion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Via = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViasAdministracion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Banos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProximoBano = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarnetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banos_Carnet_CarnetId",
                        column: x => x.CarnetId,
                        principalTable: "Carnet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Desparasitaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreMedicamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProximaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarnetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desparasitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Desparasitaciones_Carnet_CarnetId",
                        column: x => x.CarnetId,
                        principalTable: "Carnet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LimpiezaProfilaxis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProximaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarnetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimpiezaProfilaxis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LimpiezaProfilaxis_Carnet_CarnetId",
                        column: x => x.CarnetId,
                        principalTable: "Carnet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vacunas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreVacuna = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProximaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarnetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacunas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacunas_Carnet_CarnetId",
                        column: x => x.CarnetId,
                        principalTable: "Carnet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesMedicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistorialReproductivoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesMedicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesMedicos_HistorialesReproductivos_HistorialReproductivoId",
                        column: x => x.HistorialReproductivoId,
                        principalTable: "HistorialesReproductivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExploracionesFisicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    TemperaturaCorporal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    FrecuenciaCardiaca = table.Column<int>(type: "int", nullable: true),
                    FrecuenciaRespiratoria = table.Column<int>(type: "int", nullable: true),
                    LlenadoCapilar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TemperaturaRectal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CondicionCorporalId = table.Column<int>(type: "int", nullable: true),
                    EstadoGeneralId = table.Column<int>(type: "int", nullable: true),
                    MucosasId = table.Column<int>(type: "int", nullable: true),
                    HidratacionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExploracionesFisicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExploracionesFisicas_CondicionesCorporal_CondicionCorporalId",
                        column: x => x.CondicionCorporalId,
                        principalTable: "CondicionesCorporal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExploracionesFisicas_EstadosGeneral_EstadoGeneralId",
                        column: x => x.EstadoGeneralId,
                        principalTable: "EstadosGeneral",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExploracionesFisicas_Hidrataciones_HidratacionId",
                        column: x => x.HidratacionId,
                        principalTable: "Hidrataciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExploracionesFisicas_Mucosas_MucosasId",
                        column: x => x.MucosasId,
                        principalTable: "Mucosas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PruebasLaboratorios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArchivoResultado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoPruebaId = table.Column<int>(type: "int", nullable: true),
                    DiagnosticoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PruebasLaboratorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PruebasLaboratorios_Diagnosticos_DiagnosticoId",
                        column: x => x.DiagnosticoId,
                        principalTable: "Diagnosticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PruebasLaboratorios_TiposPruebas_TipoPruebaId",
                        column: x => x.TipoPruebaId,
                        principalTable: "TiposPruebas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tratamientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Recomendaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoTratamientoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tratamientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tratamientos_TiposTratamientos_TipoTratamientoId",
                        column: x => x.TipoTratamientoId,
                        principalTable: "TiposTratamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimerApellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SegundoApellido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Edad = table.Column<int>(type: "int", nullable: true),
                    NumeroIdentificacion = table.Column<int>(type: "int", nullable: true),
                    TipoUsuarioId = table.Column<int>(type: "int", nullable: true),
                    DireccionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personas_Direcciones_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "Direcciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_TiposUsuarios_TipoUsuarioId",
                        column: x => x.TipoUsuarioId,
                        principalTable: "TiposUsuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComportamientosSocializacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompProblematico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistorialMedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NivelActividadId = table.Column<int>(type: "int", nullable: false),
                    ReaccionPersonaId = table.Column<int>(type: "int", nullable: false),
                    ReaccionAnimalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComportamientosSocializacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComportamientosSocializacion_HistorialesMedicos_HistorialMedicoId",
                        column: x => x.HistorialMedicoId,
                        principalTable: "HistorialesMedicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComportamientosSocializacion_NivelActividades_NivelActividadId",
                        column: x => x.NivelActividadId,
                        principalTable: "NivelActividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComportamientosSocializacion_ReaccionesSociales_ReaccionAnimalId",
                        column: x => x.ReaccionAnimalId,
                        principalTable: "ReaccionesSociales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComportamientosSocializacion_ReaccionesSociales_ReaccionPersonaId",
                        column: x => x.ReaccionPersonaId,
                        principalTable: "ReaccionesSociales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEnfermedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaDiagnostico = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HistorialMedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnfermedadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEnfermedades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEnfermedades_Enfermedades_EnfermedadId",
                        column: x => x.EnfermedadId,
                        principalTable: "Enfermedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEnfermedades_HistorialesMedicos_HistorialMedicoId",
                        column: x => x.HistorialMedicoId,
                        principalTable: "HistorialesMedicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Frecuencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duracion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViaAdministracionId = table.Column<int>(type: "int", nullable: false),
                    TratamientoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicamentos_Tratamientos_TratamientoId",
                        column: x => x.TratamientoId,
                        principalTable: "Tratamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicamentos_ViasAdministracion_ViaAdministracionId",
                        column: x => x.ViaAdministracionId,
                        principalTable: "ViasAdministracion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recetas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URLArchivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TratamientoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recetas_Tratamientos_TratamientoId",
                        column: x => x.TratamientoId,
                        principalTable: "Tratamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Duenos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duenos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Duenos_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veterinarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CedulaProfesional = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Horarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterinarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veterinarios_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mascotas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Especie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Raza = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Caracteristicas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Edad = table.Column<int>(type: "int", nullable: true),
                    Imagen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuenoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CarnetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HistorialMedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mascotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mascotas_Carnet_CarnetId",
                        column: x => x.CarnetId,
                        principalTable: "Carnet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mascotas_Duenos_DuenoId",
                        column: x => x.DuenoId,
                        principalTable: "Duenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mascotas_HistorialesMedicos_HistorialMedicoId",
                        column: x => x.HistorialMedicoId,
                        principalTable: "HistorialesMedicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cirugias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoId = table.Column<int>(type: "int", nullable: false),
                    VeterinarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HistorialMedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cirugias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cirugias_HistorialesMedicos_HistorialMedicoId",
                        column: x => x.HistorialMedicoId,
                        principalTable: "HistorialesMedicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cirugias_TiposCirugias_TipoId",
                        column: x => x.TipoId,
                        principalTable: "TiposCirugias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cirugias_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreClinica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SitioWeb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionId = table.Column<int>(type: "int", nullable: true),
                    VeterinarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clinicas_Direcciones_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "Direcciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clinicas_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultasMedicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaConsulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InicioSintomas = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MotivoConsulta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecetaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MascotaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VeterinarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoServicioId = table.Column<int>(type: "int", nullable: false),
                    ExploracionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiagnosticoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultasMedicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultasMedicas_Diagnosticos_DiagnosticoId",
                        column: x => x.DiagnosticoId,
                        principalTable: "Diagnosticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultasMedicas_ExploracionesFisicas_ExploracionId",
                        column: x => x.ExploracionId,
                        principalTable: "ExploracionesFisicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultasMedicas_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultasMedicas_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultasMedicas_TiposServicios_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultasMedicas_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventosCalendario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pendiente = table.Column<bool>(type: "bit", nullable: false),
                    TipoServicioId = table.Column<int>(type: "int", nullable: false),
                    MascotaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosCalendario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosCalendario_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventosCalendario_TiposServicios_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvolucionesClinicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotasMedico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsultaMedicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvolucionesClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvolucionesClinicas_ConsultasMedicas_ConsultaMedicaId",
                        column: x => x.ConsultaMedicaId,
                        principalTable: "ConsultasMedicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SintomasActuales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DescripcionSintomas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuracionCantidad = table.Column<int>(type: "int", nullable: true),
                    UnidadTiempoId = table.Column<int>(type: "int", nullable: true),
                    ComportamientoId = table.Column<int>(type: "int", nullable: true),
                    ConsultaMedicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SintomasActuales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SintomasActuales_Comportamientos_ComportamientoId",
                        column: x => x.ComportamientoId,
                        principalTable: "Comportamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SintomasActuales_ConsultasMedicas_ConsultaMedicaId",
                        column: x => x.ConsultaMedicaId,
                        principalTable: "ConsultasMedicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SintomasActuales_UnidadesTiempo_UnidadTiempoId",
                        column: x => x.UnidadTiempoId,
                        principalTable: "UnidadesTiempo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvidenciasEvolucion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    URLArchivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvolucionClinicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidenciasEvolucion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvidenciasEvolucion_EvolucionesClinicas_EvolucionClinicaId",
                        column: x => x.EvolucionClinicaId,
                        principalTable: "EvolucionesClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvidenciasEvolucion_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Banos_CarnetId",
                table: "Banos",
                column: "CarnetId");

            migrationBuilder.CreateIndex(
                name: "IX_Cirugias_HistorialMedicoId",
                table: "Cirugias",
                column: "HistorialMedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cirugias_TipoId",
                table: "Cirugias",
                column: "TipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cirugias_VeterinarioId",
                table: "Cirugias",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_DireccionId",
                table: "Clinicas",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_VeterinarioId",
                table: "Clinicas",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ComportamientosSocializacion_HistorialMedicoId",
                table: "ComportamientosSocializacion",
                column: "HistorialMedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComportamientosSocializacion_NivelActividadId",
                table: "ComportamientosSocializacion",
                column: "NivelActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_ComportamientosSocializacion_ReaccionAnimalId",
                table: "ComportamientosSocializacion",
                column: "ReaccionAnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_ComportamientosSocializacion_ReaccionPersonaId",
                table: "ComportamientosSocializacion",
                column: "ReaccionPersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasMedicas_DiagnosticoId",
                table: "ConsultasMedicas",
                column: "DiagnosticoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasMedicas_ExploracionId",
                table: "ConsultasMedicas",
                column: "ExploracionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasMedicas_MascotaId",
                table: "ConsultasMedicas",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasMedicas_RecetaId",
                table: "ConsultasMedicas",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasMedicas_TipoServicioId",
                table: "ConsultasMedicas",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasMedicas_VeterinarioId",
                table: "ConsultasMedicas",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Desparasitaciones_CarnetId",
                table: "Desparasitaciones",
                column: "CarnetId");

            migrationBuilder.CreateIndex(
                name: "IX_Duenos_PersonaId",
                table: "Duenos",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosCalendario_MascotaId",
                table: "EventosCalendario",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosCalendario_TipoServicioId",
                table: "EventosCalendario",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenciasEvolucion_EvolucionClinicaId",
                table: "EvidenciasEvolucion",
                column: "EvolucionClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenciasEvolucion_PersonaId",
                table: "EvidenciasEvolucion",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_EvolucionesClinicas_ConsultaMedicaId",
                table: "EvolucionesClinicas",
                column: "ConsultaMedicaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExploracionesFisicas_CondicionCorporalId",
                table: "ExploracionesFisicas",
                column: "CondicionCorporalId");

            migrationBuilder.CreateIndex(
                name: "IX_ExploracionesFisicas_EstadoGeneralId",
                table: "ExploracionesFisicas",
                column: "EstadoGeneralId");

            migrationBuilder.CreateIndex(
                name: "IX_ExploracionesFisicas_HidratacionId",
                table: "ExploracionesFisicas",
                column: "HidratacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExploracionesFisicas_MucosasId",
                table: "ExploracionesFisicas",
                column: "MucosasId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEnfermedades_EnfermedadId",
                table: "HistorialEnfermedades",
                column: "EnfermedadId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEnfermedades_HistorialMedicoId",
                table: "HistorialEnfermedades",
                column: "HistorialMedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesMedicos_HistorialReproductivoId",
                table: "HistorialesMedicos",
                column: "HistorialReproductivoId");

            migrationBuilder.CreateIndex(
                name: "IX_LimpiezaProfilaxis_CarnetId",
                table: "LimpiezaProfilaxis",
                column: "CarnetId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_CarnetId",
                table: "Mascotas",
                column: "CarnetId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_DuenoId",
                table: "Mascotas",
                column: "DuenoId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_HistorialMedicoId",
                table: "Mascotas",
                column: "HistorialMedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_TratamientoId",
                table: "Medicamentos",
                column: "TratamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_ViaAdministracionId",
                table: "Medicamentos",
                column: "ViaAdministracionId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_DireccionId",
                table: "Personas",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_TipoUsuarioId",
                table: "Personas",
                column: "TipoUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PruebasLaboratorios_DiagnosticoId",
                table: "PruebasLaboratorios",
                column: "DiagnosticoId");

            migrationBuilder.CreateIndex(
                name: "IX_PruebasLaboratorios_TipoPruebaId",
                table: "PruebasLaboratorios",
                column: "TipoPruebaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_TratamientoId",
                table: "Recetas",
                column: "TratamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_SintomasActuales_ComportamientoId",
                table: "SintomasActuales",
                column: "ComportamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_SintomasActuales_ConsultaMedicaId",
                table: "SintomasActuales",
                column: "ConsultaMedicaId");

            migrationBuilder.CreateIndex(
                name: "IX_SintomasActuales_UnidadTiempoId",
                table: "SintomasActuales",
                column: "UnidadTiempoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tratamientos_TipoTratamientoId",
                table: "Tratamientos",
                column: "TipoTratamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacunas_CarnetId",
                table: "Vacunas",
                column: "CarnetId");

            migrationBuilder.CreateIndex(
                name: "IX_Veterinarios_PersonaId",
                table: "Veterinarios",
                column: "PersonaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergias");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Banos");

            migrationBuilder.DropTable(
                name: "Cirugias");

            migrationBuilder.DropTable(
                name: "Clinicas");

            migrationBuilder.DropTable(
                name: "ComportamientosSocializacion");

            migrationBuilder.DropTable(
                name: "Desparasitaciones");

            migrationBuilder.DropTable(
                name: "EventosCalendario");

            migrationBuilder.DropTable(
                name: "EvidenciasEvolucion");

            migrationBuilder.DropTable(
                name: "HistorialEnfermedades");

            migrationBuilder.DropTable(
                name: "LimpiezaProfilaxis");

            migrationBuilder.DropTable(
                name: "Medicamentos");

            migrationBuilder.DropTable(
                name: "PruebasLaboratorios");

            migrationBuilder.DropTable(
                name: "SintomasActuales");

            migrationBuilder.DropTable(
                name: "Vacunas");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TiposCirugias");

            migrationBuilder.DropTable(
                name: "NivelActividades");

            migrationBuilder.DropTable(
                name: "ReaccionesSociales");

            migrationBuilder.DropTable(
                name: "EvolucionesClinicas");

            migrationBuilder.DropTable(
                name: "Enfermedades");

            migrationBuilder.DropTable(
                name: "ViasAdministracion");

            migrationBuilder.DropTable(
                name: "TiposPruebas");

            migrationBuilder.DropTable(
                name: "Comportamientos");

            migrationBuilder.DropTable(
                name: "UnidadesTiempo");

            migrationBuilder.DropTable(
                name: "ConsultasMedicas");

            migrationBuilder.DropTable(
                name: "Diagnosticos");

            migrationBuilder.DropTable(
                name: "ExploracionesFisicas");

            migrationBuilder.DropTable(
                name: "Mascotas");

            migrationBuilder.DropTable(
                name: "Recetas");

            migrationBuilder.DropTable(
                name: "TiposServicios");

            migrationBuilder.DropTable(
                name: "Veterinarios");

            migrationBuilder.DropTable(
                name: "CondicionesCorporal");

            migrationBuilder.DropTable(
                name: "EstadosGeneral");

            migrationBuilder.DropTable(
                name: "Hidrataciones");

            migrationBuilder.DropTable(
                name: "Mucosas");

            migrationBuilder.DropTable(
                name: "Carnet");

            migrationBuilder.DropTable(
                name: "Duenos");

            migrationBuilder.DropTable(
                name: "HistorialesMedicos");

            migrationBuilder.DropTable(
                name: "Tratamientos");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "HistorialesReproductivos");

            migrationBuilder.DropTable(
                name: "TiposTratamientos");

            migrationBuilder.DropTable(
                name: "Direcciones");

            migrationBuilder.DropTable(
                name: "TiposUsuarios");
        }
    }
}

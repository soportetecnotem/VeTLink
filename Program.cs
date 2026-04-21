using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VeTLink.Data;
using VeTLink.DTOs.Llave;
using VeTLink.Models;
using VeTLink.Services;
using VeTLink.Utilidades;

var builder = WebApplication.CreateBuilder(args);

// Inicio area de servicios
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCors =>
    {
        opcionesCors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

//AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

//Configuración de EF + Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IServicioLlaves, ServicioLlaves>();

builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

builder.Configuration
    .AddUserSecrets<Program>();

var azureKey = builder.Configuration["AzureKey"];

//Configuración de JWT
//var jwtKey = builder.Configuration["Jwt:Key"];
//var jwtIssuer = builder.Configuration["Jwt:Issuer"];
//var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["LlaveJWT"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Apis VeTLink",
        Description = "Apis para historiales medicos web y app"
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });

});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorList = context.ModelState
    .SelectMany(x => x.Value!.Errors)
    .Select(e => e.ErrorMessage)
    .ToList();

        var customResponse = new
        {
            Status = false,
            Message = errorList
        };

        return new BadRequestObjectResult(customResponse);
    };
});

//Configuracion de correo
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddOptions<LimitarPeticionesDTO>()
    .Bind(builder.Configuration.GetSection(LimitarPeticionesDTO.Seccion))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

//APLICAR MIGRACIONES AUTOMÁTICAMENTE EN PRODUCCIÓN
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
}

//Inicio area de middlewares
app.UseStaticFiles(); // Necesario para acceder a archivos desde wwwroot

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseLimitarPeticiones(); //middleware personalizado para limitar peticiones por día a usuarios gratuitos

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
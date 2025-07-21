using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using VeTLink.Data;
using VeTLink.Models;

var builder = WebApplication.CreateBuilder(args);

// Inicio area de servicios
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCors =>
    {
        opcionesCors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

//builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
opciones.UseSqlServer("name=DefaultConnection"));

////Codigo para usuarios con identity
//builder.Services.AddIdentityCore<Usuario>()
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

//builder.Services.AddScoped<UserManager<Usuario>>();
//builder.Services.AddScoped<SignInManager<Usuario>>();

builder.Services.AddHttpContextAccessor();

//builder.Services.AddAuthentication().AddJwtBearer(opciones =>
//{
//    opciones.MapInboundClaims = false;

//    opciones.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey =
//        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["LlaveJWT"]!)),
//        ClockSkew = TimeSpan.Zero
//    };
//});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", politica => politica.RequireClaim("Admin"));

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

    //opciones.OperationFilter<FiltroAutorizacion>();

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


var app = builder.Build();

//Inicio area de middlewares
app.UseStaticFiles(); // Necesario para acceder a archivos desde wwwroot

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
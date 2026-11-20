using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Veterinaria.API.Services;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Repositories;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services;
using Veterinaria.Logica.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// los enums se mandan como texto en el json
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

// SWAGGER CON EL BOTON AUTHORIZE PARA PROBAR CON EL TOKEN
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] { }
        }
    });
});

// INYECCION DE DEPENDENCIAS PARA LA BASE DE DATOS
builder.Services.AddDbContext<VeterinariaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// INYECCION DE DEPENDENCIAS PARA LOS REPOSITORIOS
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMascotaRepository, MascotaRepository>();
builder.Services.AddScoped<ITipoMascotaRepository, TipoMascotaRepository>();
builder.Services.AddScoped<ITipoAtencionRepository, TipoAtencionRepository>();
builder.Services.AddScoped<IAtencionRepository, AtencionRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// INYECCION DE DEPENDENCIAS PARA LOS SERVICIOS
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IMascotaService, MascotaService>();
builder.Services.AddScoped<ITipoMascotaService, TipoMascotaService>();
builder.Services.AddScoped<ITipoAtencionService, TipoAtencionService>();
builder.Services.AddScoped<IAtencionService, AtencionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<TokenService>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// CORS para que la pagina web pueda llamar a la api
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
        policy.WithOrigins(builder.Configuration["Cors:OrigenWeb"]!)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

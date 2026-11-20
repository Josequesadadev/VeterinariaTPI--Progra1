using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Veterinaria.API.Services;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services;
using Veterinaria.Logica.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Los enums viajan como texto ("Masculino", "Admin") en vez de numeros
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// SWAGGER con boton "Authorize" para probar con el token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Veterinaria API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegar solo el token (sin la palabra Bearer)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// INYECCION DE DEPENDENCIAS PARA LA BASE DE DATOS
builder.Services.AddDbContext<VeterinariaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// INYECCION DE DEPENDENCIAS PARA LOS REPOSITORIOS
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMascotaRepository, MascotaRepository>();
builder.Services.AddScoped<IAtencionRepository, AtencionRepository>();
builder.Services.AddScoped<ICatalogoRepository<TipoMascota>, TipoMascotaRepository>();
builder.Services.AddScoped<ICatalogoRepository<TipoAtencion>, TipoAtencionRepository>();

// INYECCION DE DEPENDENCIAS PARA LOS SERVICIOS (reglas de negocio)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IMascotaService, MascotaService>();
builder.Services.AddScoped<IAtencionService, AtencionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICatalogoService<TipoMascota>, CatalogoService<TipoMascota>>();
builder.Services.AddScoped<ICatalogoService<TipoAtencion>, CatalogoService<TipoAtencion>>();
builder.Services.AddScoped<TokenService>();

// AUTENTICACION CON JWT: valida firma, emisor, audiencia y vencimiento del token
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// CORS: permite que la pagina web (otro puerto) consuma la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirWeb", policy =>
        policy.WithOrigins(builder.Configuration["Cors:OrigenWeb"]!)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Crea los usuarios iniciales (admin y empleado) si la tabla esta vacia
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<IAuthService>().CrearUsuariosIniciales();
}

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

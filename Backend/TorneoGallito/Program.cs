using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Middleware;
using TorneoGallito.Services;
using TorneoGallito.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de controladores y respuestas de error
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errores = context.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());
            return new BadRequestObjectResult(new MensajeResponseDTO("Datos inválidos.", errores));
        };
    });

// 2. Conexión a PostgreSQL con el formato Snake Case que pide el equipo
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CadenaConexion"))
           .UseSnakeCaseNamingConvention());

// 3. Inyección de los servicios de la arquitectura del torneo[cite: 6]
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IRobotService, RobotService>();
builder.Services.AddScoped<ITorneoService, TorneoService>();
builder.Services.AddScoped<ICombateService, CombateService>();
builder.Services.AddScoped<IPublicService, PublicService>();

// 4. Tu Seguridad: Configuración estricta del Token JWT
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();

// 5. Activación de Swagger limpio[cite: 6]
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 6. Configuración de CORS para Angular[cite: 6]
builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("Cors:Origenes").Get<string[]>() ?? [])
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

// Pipeline de peticiones HTTP
app.UseMiddleware<ManejoErroresMiddleware>();

// Aquí se enciende la interfaz de Swagger web[cite: 6]
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");
app.UseAuthentication(); // 1. Lee el token
app.UseAuthorization();  // 2. Revisa permisos

app.MapControllers();
app.Run();
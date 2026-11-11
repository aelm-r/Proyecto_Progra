using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Middleware;
using TorneoGallito.Services;
using TorneoGallito.Services.Interfaces;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CadenaConexion"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IRobotService, RobotService>();
builder.Services.AddScoped<ITorneoService, TorneoService>();
builder.Services.AddScoped<ICombateService, CombateService>();
builder.Services.AddScoped<IPublicService, PublicService>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errores = new Dictionary<string, string[]>();

            foreach (var campo in context.ModelState)
            {
                if (campo.Value.Errors.Count > 0)
                {
                    var mensajes = new List<string>();
                    foreach (var error in campo.Value.Errors)
                    {
                        mensajes.Add(error.ErrorMessage);
                    }
                    errores.Add(campo.Key, mensajes.ToArray());
                }
            }

            return new BadRequestObjectResult(new MensajeResponseDTO("Datos inválidos.", errores));
        };
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = "rol"
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Torneo Robot Gallito", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega solo el token (sin la palabra Bearer)"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() }
    });
});

var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>();
if (origenes == null)
{
    origenes = new string[] { "http://localhost:4200" };
}

builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(origenes)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ManejoErroresMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

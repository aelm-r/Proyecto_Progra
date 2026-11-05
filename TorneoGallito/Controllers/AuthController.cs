using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Request;

namespace TorneoGallito.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            // 1. Busca el usuario en la base de datos
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Username == loginDto.Username && u.PasswordHash == loginDto.Password);

            if (usuario == null || !usuario.Activo)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas o usuario inactivo." });
            }

            // 2. Genera el Token JWT (Vigencia de 8 horas según requerimientos)
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim("RolId", usuario.RolId.ToString())
            };

            int horasExpiracion = _config.GetValue<int>("Jwt:HorasExpiracion");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(horasExpiracion),
                signingCredentials: credentials);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = jwtToken });
        }
    }
}
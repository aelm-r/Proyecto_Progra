using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Models;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public LoginResponseDTO GenerarToken(Usuario usuario)
        {
            var llaveSegura = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credenciales = new SigningCredentials(llaveSegura, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim("id", usuario.Id.ToString()),
                new Claim("username", usuario.Username),
                new Claim("rol", usuario.Rol.Nombre)
            };

            var expiracion = DateTime.UtcNow.AddHours(_config.GetValue<int>("Jwt:HorasExpiracion"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiracion,
                signingCredentials: credenciales
            );

            return new LoginResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion,
                Username = usuario.Username,
                NombreCompleto = usuario.NombreCompleto,
                Rol = usuario.Rol.Nombre
            };
        }
    }
}

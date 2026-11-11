using Microsoft.EntityFrameworkCore;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public LoginResponseDTO Login(LoginDTO dto)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Username == dto.Username);

            if (usuario == null || !usuario.Activo || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrecta");
            }

            return _tokenService.GenerarToken(usuario);
        }
    }
}

using Microsoft.AspNetCore.Identity.Data;
using System.Runtime.CompilerServices;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Services.Interface;

namespace TorneoGallito.Services
{
    public class AuthServices : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthServices(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async LoginResponsableDTO Login(LoginDTO dto)
        {
           var usuario = await _context.Usuarios
                .Include(u  => u.Rol)
                .FirstOrDefault(u => u.Username == username);

            if (usuario == null || !BCrypt.Net.Verify(password, usuario.PasswordHash))
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos")
            }

            return _tokenService.GenerarToken(usuario);

            throw new NotImplementedException();
        }
    }
}

using TorneoGallito.DTOs.Responses;
using TorneoGallito.Models;

namespace TorneoGallito.Services.Interfaces
{
    public interface ITokenService
    {
        LoginResponseDTO GenerarToken(Usuario usuario);
    }
}

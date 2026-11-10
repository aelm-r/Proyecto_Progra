using TorneoGallito.DTOs.Responses;
using TorneoGallito.Models;

namespace TorneoGallito.Services.Interface
{
    public class ITokenService
    {
        string GenerarToken(Usuario usuario);
    }
}

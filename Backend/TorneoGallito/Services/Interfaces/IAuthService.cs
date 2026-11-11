using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;

namespace TorneoGallito.Services.Interfaces
{
    public interface IAuthService
    {
        LoginResponseDTO Login(LoginDTO dto);
    }
}

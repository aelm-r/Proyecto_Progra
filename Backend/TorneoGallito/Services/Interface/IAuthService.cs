using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;

namespace TorneoGallito.Services.Interface
{
    public interface IAuthService
    {
        LoginResponseDTO Login(LoginDTO dto);
    }
}

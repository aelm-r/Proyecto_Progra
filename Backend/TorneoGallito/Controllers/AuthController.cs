using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<LoginResponseDTO> Login([FromBody] LoginDTO login)
        {
            var respuesta = _authService.Login(login);
            return Ok(respuesta);
        }
    }
}

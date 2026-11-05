using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatorio.")]
        public string Password { get; set; } = string.Empty;
    }
}

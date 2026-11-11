using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class RobotUpdateDTO
    {
        [Required(ErrorMessage = "El nombre del robot es obligatorio.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre del robot debe tener hasta 30 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Url(ErrorMessage = "La foto debe ser una URL válida.")]
        [StringLength(300, ErrorMessage = "La URL de la foto no puede pasar de 300 caracteres.")]
        public string? FotoUrl { get; set; }
    }
}

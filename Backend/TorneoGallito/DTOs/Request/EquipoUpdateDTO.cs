using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class EquipoUpdateDTO
    {
        [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 30 caracteres.")]

        public string Nombre { get; set; } = string.Empty;

        [StringLength(120, ErrorMessage = "El lema no puede pasar de 120 caracteres.")]

        public string? Lema { get; set; }

        [Required(ErrorMessage = "El color es obligatorio.")]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "El color debe tener el formato válido")]

        public string ColorHex { get; set; } = string.Empty;

        [Url(ErrorMessage = "El logo debe ser una URL válida.")]
        [StringLength(300, ErrorMessage = "La URL del logo no puede pasar de 300 caracteres.")]
        public string? LogoUrl { get; set; }
    }
}

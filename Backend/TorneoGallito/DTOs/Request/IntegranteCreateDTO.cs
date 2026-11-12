using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class IntegranteCreateDTO
    {
        [Required(ErrorMessage = "El nombre del integrante es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]


        public string NombreCompleto { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "El carné es obligatorio.")]
        [RegularExpression("^[0-9]{6,15}$", ErrorMessage = "El carné solo lleva números.")]


        public string Carne { get; set; } = string.Empty;
    }
}

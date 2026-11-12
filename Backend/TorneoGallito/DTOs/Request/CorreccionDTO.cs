using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class CorreccionDTO
    {
        public int? GanadorId { get; set; }

        public string? TipoResultado { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "El motivo debe tener entre 1 y 300 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}

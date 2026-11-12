using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class AmonestacionCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Indicar el equipo amonestado.")]
        public int EquipoId { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El motivo debe tener entre 1 y 200 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}

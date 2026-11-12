using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class DescalificacionDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Indica el equipo descalificado.")]
        public int EquipoId { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "El motivo debe tener entre 1 y 300 caracteres.")]
        public string Motivo { get; set; } = string.Empty;

        public bool DelTorneo { get; set; }
    }
}

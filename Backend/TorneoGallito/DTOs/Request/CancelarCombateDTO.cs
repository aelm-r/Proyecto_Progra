using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    // POST /api/combates/{id}/cancelar (solo Admin, combate programado)
    public class CancelarCombateDTO
    {
        // TODO: [Required][StringLength
        [Required(ErrorMessage = "El motivo es obligatorio.")]
                [StringLength(300, MinimumLength = 10)]
        public string Motivo { get; set; } = string.Empty;
    }
}

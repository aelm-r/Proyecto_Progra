using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    // PUT /api/combates/{id}/correccion (solo Admin, combate finalizado)
    public class CorreccionDTO
    {
        // TODO: Motivo [Required][StringLength(300, MinimumLength = 10)] -> regla del documento: 10 a 300 caracteres
        [Range(1, int.MaxValue)]
        public int? GanadorId { get; set; }          // null = empate (solo grupos)
        public string? TipoResultado { get; set; }
        [Required(ErrorMessage = "El motivo es obligatorio.")]
                [StringLength(300, MinimumLength = 10)]
        public string Motivo { get; set; } = string.Empty;
    }
}

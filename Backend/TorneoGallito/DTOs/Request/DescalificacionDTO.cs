using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    // POST /api/combates/{id}/descalificacion -> llama a sp_registrar_descalificado
    public class DescalificacionDTO
    {
        // TODO: EquipoId [Range(1, int.MaxValue)], Motivo [Required][StringLength(300, MinimumLength = 10)]
        [Range(1, int.MaxValue)]
        public int EquipoId { get; set; }
        [Required(ErrorMessage = "El motivo es obligatorio.")]
                [StringLength(300, MinimumLength = 10)]
        public string Motivo { get; set; } = string.Empty;
        public bool DelTorneo { get; set; } // true = pierde también todos sus combates pendientes
    }
}

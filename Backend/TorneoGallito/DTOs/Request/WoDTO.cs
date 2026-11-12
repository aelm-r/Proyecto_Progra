using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class WoDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Indica el equipo que no se presentó.")]
        public int EquipoAusenteId { get; set; }
    }
}

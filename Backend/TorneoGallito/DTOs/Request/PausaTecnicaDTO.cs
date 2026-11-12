using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    public class PausaTecnicaDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Indica el equipo que pide la pausa.")]
        public int EquipoId { get; set; }
    }
}

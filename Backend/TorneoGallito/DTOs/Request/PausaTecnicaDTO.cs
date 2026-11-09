using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    // POST /api/combates/{id}/pausa -> qué equipo pide su pausa técnica
    public class PausaTecnicaDTO
    {
        // TODO: [Range(1, int.MaxValue)]
        [Range(1, int.MaxValue)]
        public int EquipoId { get; set; }
    }
}

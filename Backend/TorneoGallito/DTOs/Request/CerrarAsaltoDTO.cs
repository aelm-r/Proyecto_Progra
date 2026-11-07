using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    // PUT /api/combates/{id}/asaltos/actual
    // GanadorId = el equipo que GANA el asalto (en autogolpe/fuera_arena/inmovil es el rival del que falló).
    // Null solo cuando el resultado es "nulo".
    public class CerrarAsaltoDTO
    {
        // TODO: [Required] en Resultado; validar en el servicio contra ResultadosAsalto.Todos
        [Required(ErrorMessage = "El resultado es obligatorio.")]
        public string Resultado { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int? GanadorId { get; set; }
    }
}

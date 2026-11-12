using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    
    public class CerrarAsaltoDTO
    {
        
        [Required(ErrorMessage = "El resultado es obligatorio.")]
        public string Resultado { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int? GanadorId { get; set; }
    }
}

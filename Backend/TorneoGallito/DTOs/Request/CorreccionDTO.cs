using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    
    public class CorreccionDTO
    {
        
        [Range(1, int.MaxValue)]
        public int? GanadorId { get; set; } 
        public string? TipoResultado { get; set; }
        [Required(ErrorMessage = "El motivo es obligatorio.")]
                [StringLength(300, MinimumLength = 10)]
        public string Motivo { get; set; } = string.Empty;
    }
}

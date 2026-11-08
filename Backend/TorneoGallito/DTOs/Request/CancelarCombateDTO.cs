using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    
    public class CancelarCombateDTO
    {
        
        [Required(ErrorMessage = "El motivo es obligatorio.")]
                [StringLength(300, MinimumLength = 10)]
        public string Motivo { get; set; } = string.Empty;
    }
}

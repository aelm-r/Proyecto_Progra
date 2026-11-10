using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    
    public class DescalificacionDTO
    {
        
        [Range(1, int.MaxValue)]
        public int EquipoId { get; set; }
        [Required(ErrorMessage = "El motivo es obligatorio.")]
                [StringLength(300, MinimumLength = 10)]
        public string Motivo { get; set; } = string.Empty;
        public bool DelTorneo { get; set; } 
    }
}

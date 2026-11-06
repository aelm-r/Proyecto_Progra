using System.ComponentModel.DataAnnotations;

namespace TorneoGallito.DTOs.Request
{
    // está en /api/robots/{id}/estado para  (Admin o Juez)
    public class RobotEstadoDTO
    {
        
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression("^(activo|en_reparacion|descalificado)$",
            ErrorMessage = "El estado debe ser: activo, en_reparacion o descalificado.")]
        
        
        public string Estado { get; set; } = string.Empty;
    }
}

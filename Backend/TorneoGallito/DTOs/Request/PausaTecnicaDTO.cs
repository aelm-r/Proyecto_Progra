using System.ComponentModel.DataAnnotations;
namespace TorneoGallito.DTOs.Request
{
    
    public class PausaTecnicaDTO
    {
   
        [Range(1, int.MaxValue)]
        public int EquipoId { get; set; }
    }
}

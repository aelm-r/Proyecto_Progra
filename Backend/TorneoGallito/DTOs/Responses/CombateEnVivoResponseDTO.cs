namespace TorneoGallito.DTOs.Responses
{
    public class CombateEnVivoResponseDTO
    {
        public int CombateId { get; set; }
        public int NumeroOrden { get; set; }
        public string Fase { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;   
        public EquipoResumenDTO EquipoA { get; set; } = new();
        public EquipoResumenDTO EquipoB { get; set; } = new();
        public int AsaltosA { get; set; }
        public int AsaltosB { get; set; }
        public string Marcador { get; set; } = string.Empty;
        public AsaltoResponseDTO? AsaltoActual { get; set; } 
        public int? PausaEquipoId { get; set; }              
        public DateTime? PausaInicio { get; set; }          
    }
}

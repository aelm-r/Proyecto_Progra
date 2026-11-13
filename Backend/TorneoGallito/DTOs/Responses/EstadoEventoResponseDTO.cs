namespace TorneoGallito.DTOs.Responses
{
    public class EstadoEventoResponseDTO
    {
        public DateTime HoraServidor { get; set; }              
        public string Fase { get; set; } = string.Empty;
        public string PantallaActual { get; set; } = string.Empty;
        public List<EquipoResponseDTO> Equipos { get; set; } = new();   
        public SorteoResponseDTO Sorteo { get; set; } = new();          
        public CombateEnVivoResponseDTO? CombateEnCurso { get; set; }   
        public CombateResponseDTO? SiguienteCombate { get; set; }      
        public CombateResponseDTO? UltimoFinalizado { get; set; }      
        public List<GrupoResponseDTO> Grupos { get; set; } = new();     
        public List<LlaveResponseDTO> Bracket { get; set; } = new();    
        public PodioResponseDTO? Podio { get; set; }                    
        public int TotalCombates { get; set; } = 16;                    
    }
}

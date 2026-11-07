namespace TorneoGallito.DTOs.Responses
{
    // GET /api/public/estado-evento: TODO lo que la TV necesita en UNA llamada (cada 2 s).
    // La TV decide sola qué vista mostrar con estos datos; nunca calcula ganadores ni puntos.
    public class EstadoEventoResponseDTO
    {
        public DateTime HoraServidor { get; set; }                 // la TV sincroniza su cronómetro con esto
        public string Fase { get; set; } = string.Empty;           // fn_fase_evento()
        public List<EquipoResponseDTO> Equipos { get; set; } = new();   // vista Bienvenida
        public SorteoResponseDTO Sorteo { get; set; } = new();          // vista Sorteo
        public CombateEnVivoResponseDTO? CombateEnCurso { get; set; }   // vista Combate en vivo
        public CombateResponseDTO? SiguienteCombate { get; set; }       // vista Próximo combate
        public CombateResponseDTO? UltimoFinalizado { get; set; }       // vista Resultado (15 s tras FechaFin)
        public List<GrupoResponseDTO> Grupos { get; set; } = new();     // vista Rotación
        public List<LlaveResponseDTO> Bracket { get; set; } = new();    // vista Rotación
        public PodioResponseDTO? Podio { get; set; }                    // vista Campeón
        public int TotalCombates { get; set; } = 16;                    // «Combate N de 16»
    }
}

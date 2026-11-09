namespace TorneoGallito.DTOs.Responses
{
    // Combate en curso para la TV y el panel del juez (sale de fn_combate_en_curso)
    public class CombateEnVivoResponseDTO
    {
        public int CombateId { get; set; }
        public int NumeroOrden { get; set; }
        public string Fase { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;   // en_curso o en_pausa
        public EquipoResumenDTO EquipoA { get; set; } = new();
        public EquipoResumenDTO EquipoB { get; set; } = new();
        public int AsaltosA { get; set; }
        public int AsaltosB { get; set; }
        public string Marcador { get; set; } = string.Empty;
        public AsaltoResponseDTO? AsaltoActual { get; set; } // null = entre asaltos
        public int? PausaEquipoId { get; set; }              // quién pidió la pausa activa
        public DateTime? PausaInicio { get; set; }           // la TV cuenta los 2 minutos
    }
}

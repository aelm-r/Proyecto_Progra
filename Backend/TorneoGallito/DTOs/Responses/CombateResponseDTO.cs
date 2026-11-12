namespace TorneoGallito.DTOs.Responses
{
    public class CombateResponseDTO
    {
        public int Id { get; set; }
        public int NumeroOrden { get; set; }
        public string Fase { get; set; } = string.Empty;
        public string? Llave { get; set; }
        public string? Grupo { get; set; }
        public string Estado { get; set; } = string.Empty;
        public EquipoResumenDTO? EquipoA { get; set; }  
        public EquipoResumenDTO? EquipoB { get; set; }
        public int? GanadorId { get; set; }
        public string? TipoResultado { get; set; }
        public string Marcador { get; set; } = "0 – 0"; 
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public List<AsaltoResponseDTO> Asaltos { get; set; } = new();
    }
}

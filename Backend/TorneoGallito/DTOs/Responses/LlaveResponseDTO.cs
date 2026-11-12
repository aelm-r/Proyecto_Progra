namespace TorneoGallito.DTOs.Responses
{
    public class LlaveResponseDTO
    {
        public int CombateId { get; set; }
        public int NumeroOrden { get; set; }
        public string Fase { get; set; } = string.Empty;
        public string? Llave { get; set; }
        public int? EquipoAId { get; set; }
        public string EquipoA { get; set; } = string.Empty;   
        public int? EquipoBId { get; set; }
        public string EquipoB { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Marcador { get; set; }
        public int? GanadorId { get; set; }
    }
}

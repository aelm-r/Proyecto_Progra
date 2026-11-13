namespace TorneoGallito.DTOs.Responses
{
    public class LlaveResponseDTO
    {
        public int CombateId { get; set; }
        public int NumeroOrden { get; set; }
        public string Fase { get; set; } = string.Empty;
        public string? Llave { get; set; }
        public int? EquipoAId { get; set; }
        public string EquipoANombre { get; set; } = string.Empty;
        public EquipoResumenDTO? EquipoA { get; set; }
        public int? EquipoBId { get; set; }
        public string EquipoBNombre { get; set; } = string.Empty;
        public EquipoResumenDTO? EquipoB { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Marcador { get; set; }
        public int? GanadorId { get; set; }
    }
}

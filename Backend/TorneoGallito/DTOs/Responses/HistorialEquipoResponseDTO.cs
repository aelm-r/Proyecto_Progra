namespace TorneoGallito.DTOs.Responses
{
    public class HistorialEquipoResponseDTO
    {
        public int CombateId { get; set; }
        public int NumeroOrden { get; set; }
        public string Fase { get; set; } = string.Empty;
        public string Rival { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
        public string? Marcador { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}

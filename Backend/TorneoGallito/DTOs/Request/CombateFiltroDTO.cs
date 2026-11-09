namespace TorneoGallito.DTOs.Request
{
    // Query string de GET /api/combates?fase=grupos&estado=programado
    public class CombateFiltroDTO
    {
        public string? Fase { get; set; }
        public string? Estado { get; set; }
    }
}

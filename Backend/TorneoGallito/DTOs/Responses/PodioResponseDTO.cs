namespace TorneoGallito.DTOs.Responses
{
    // Vista «Campeón» de la TV (solo cuando el torneo está finalizado)
    public class PodioResponseDTO
    {
        public EquipoResumenDTO? Primero { get; set; }
        public EquipoResumenDTO? Segundo { get; set; }
        public EquipoResumenDTO? Tercero { get; set; }
    }
}

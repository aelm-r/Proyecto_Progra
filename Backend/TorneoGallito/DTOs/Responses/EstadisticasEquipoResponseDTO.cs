namespace TorneoGallito.DTOs.Responses
{
    public class EstadisticasEquipoResponseDTO
    {
        public EquipoResumenDTO Equipo { get; set; } = new();
        public decimal TiempoPromedioReventonSegundos { get; set; }
    }
}

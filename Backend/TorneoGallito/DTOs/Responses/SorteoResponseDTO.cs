namespace TorneoGallito.DTOs.Responses
{
    public class SorteoResponseDTO
    {
        public bool Realizado { get; set; }
        public List<SorteoEquipoDTO> Equipos { get; set; } = new();
    }

    public class SorteoEquipoDTO
    {
        public string Grupo { get; set; } = string.Empty;
        public int PosicionSorteo { get; set; }
        public EquipoResumenDTO Equipo { get; set; } = new();
    }
}

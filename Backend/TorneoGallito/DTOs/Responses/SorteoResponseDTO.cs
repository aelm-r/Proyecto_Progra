namespace TorneoGallito.DTOs.Responses
{
    // La TV lo anima en el orden de la lista: A1, B1, A2, B2... (acordar con F1)
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

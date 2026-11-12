namespace TorneoGallito.DTOs.Responses
{
    public class GrupoResponseDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public List<PosicionResponseDTO> Posiciones { get; set; } = new();
    }
}

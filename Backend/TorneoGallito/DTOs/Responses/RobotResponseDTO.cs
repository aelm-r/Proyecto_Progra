namespace TorneoGallito.DTOs.Responses
{
    public class RobotResponseDTO
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}

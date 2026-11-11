namespace TorneoGallito.DTOs.Responses
{
    public class EquipoResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Lema { get; set; }
        public string ColorHex { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Grupo { get; set; }
        public int? PosicionSorteo { get; set; }
        public RobotResponseDTO? Robot { get; set; }
        public List<IntegranteResponseDTO> Integrantes { get; set; } = new();
    }
}

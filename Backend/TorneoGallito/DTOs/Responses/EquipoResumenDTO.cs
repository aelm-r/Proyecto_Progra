namespace TorneoGallito.DTOs.Responses
{
    public class EquipoResumenDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Robot { get; set; }
    }
}

namespace TorneoGallito.DTOs.Responses
{
    public class IntegranteResponseDTO
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Carne { get; set; } = string.Empty;
    }
}

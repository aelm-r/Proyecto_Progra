namespace TorneoGallito.DTOs.Request
{
    public class AmonestacionCreateDTO
    {
        public int EquipoId { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }
}

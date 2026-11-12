namespace TorneoGallito.DTOs.Responses
{
    public class AsaltoResponseDTO
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public bool EsMuerteSubita { get; set; }
        public string? Resultado { get; set; }       
        public int? GanadorId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int DuracionMaximaSegundos { get; set; } 
}

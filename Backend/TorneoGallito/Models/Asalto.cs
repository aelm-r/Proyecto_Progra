namespace TorneoGallito.Models
{
    
    public class Asalto
    {
        public int Id { get; set; }
        public int CombateId { get; set; }
        public int Numero { get; set; }               
        public bool EsMuerteSubita { get; set; }
        public string? Resultado { get; set; }
        public int? GanadorId { get; set; }           
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public Combate Combate { get; set; } = null!;
        public Equipo? Ganador { get; set; }
    }
}

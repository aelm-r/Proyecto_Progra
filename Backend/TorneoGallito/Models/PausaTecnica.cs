namespace TorneoGallito.Models
{
    public class PausaTecnica
    {
        public int Id { get; set; }
        public int CombateId { get; set; }
        public int EquipoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public Combate Combate { get; set; } = null!;
        public Equipo Equipo { get; set; } = null!;
    }
}

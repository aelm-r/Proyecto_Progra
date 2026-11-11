namespace TorneoGallito.Models
{
    public class Amonestacion
    {
        public int Id { get; set; }
        public int CombateId { get; set; }
        public int? AsaltoId { get; set; }
        public int EquipoId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }

        public Combate Combate { get; set; } = null!;
        public Asalto? Asalto { get; set; }
        public Equipo Equipo { get; set; } = null!;
    }
}

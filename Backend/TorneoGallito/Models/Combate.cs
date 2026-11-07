namespace TorneoGallito.Models
{
    // Tabla: combates. Los 16 los crea sp_realizar_sorteo (los 4 eliminatorios con equipos NULL).
    public class Combate
    {
        public int Id { get; set; }
        public int NumeroOrden { get; set; }          // 1 a 16
        public string Fase { get; set; } = "grupos";  // grupos, semifinal, tercer_lugar, final
        public string? Llave { get; set; }            // SF1, SF2, TERCER, FINAL (acordar con BD)
        public int? GrupoId { get; set; }             // solo fase de grupos
        public int? EquipoAId { get; set; }           // NULL hasta conocerse
        public int? EquipoBId { get; set; }
        public string Estado { get; set; } = "programado";
        public int? GanadorId { get; set; }           // NULL si empate
        public string? TipoResultado { get; set; }
        public DateTime? FechaInicio { get; set; }    // la pone el servidor
        public DateTime? FechaFin { get; set; }
        public int? ActualizadoPor { get; set; }      // usuario del token: lo usa el trigger de auditoría
        public string? MotivoCambio { get; set; }     // motivo de corrección/cancelación (acordar con BD)

        public Grupo? Grupo { get; set; }
        public Equipo? EquipoA { get; set; }
        public Equipo? EquipoB { get; set; }
        public Equipo? Ganador { get; set; }
        public Usuario? UsuarioActualizo { get; set; }

        public ICollection<Asalto> Asaltos { get; set; } = new List<Asalto>();
        public ICollection<Amonestacion> Amonestaciones { get; set; } = new List<Amonestacion>();
        public ICollection<PausaTecnica> PausasTecnicas { get; set; } = new List<PausaTecnica>();
    }
}

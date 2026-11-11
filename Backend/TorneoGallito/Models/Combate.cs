namespace TorneoGallito.Models
{
    public class Combate
    {
        public int Id { get; set; }
        public int NumeroOrden { get; set; }          
        public string Fase { get; set; } = "grupos";  
        public string? Llave { get; set; }            
        public int? GrupoId { get; set; }             
        public int? EquipoAId { get; set; }           
        public int? EquipoBId { get; set; }
        public string Estado { get; set; } = "programado";
        public int? GanadorId { get; set; }           
        public string? TipoResultado { get; set; }
        public DateTime? FechaInicio { get; set; }    
        public DateTime? FechaFin { get; set; }
        public int? ActualizadoPor { get; set; }      

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

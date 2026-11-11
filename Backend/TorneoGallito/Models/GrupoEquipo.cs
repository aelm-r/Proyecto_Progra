namespace TorneoGallito.Models
{
    public class GrupoEquipo
    {
        public int GrupoId { get; set; }
        public int EquipoId { get; set; }
        public int PosicionSorteo { get; set; } 

        public Grupo Grupo { get; set; } = null!;
        public Equipo Equipo { get; set; } = null!;
    }
}

namespace TorneoGallito.Models
{
    // Tabla: grupo_equipos. Llave compuesta (grupo_id, equipo_id). Lo llena sp_realizar_sorteo.
    public class GrupoEquipo
    {
        public int GrupoId { get; set; }
        public int EquipoId { get; set; }
        public int PosicionSorteo { get; set; } // 1 a 4 -> A1..A4, B1..B4

        public Grupo Grupo { get; set; } = null!;
        public Equipo Equipo { get; set; } = null!;
    }
}

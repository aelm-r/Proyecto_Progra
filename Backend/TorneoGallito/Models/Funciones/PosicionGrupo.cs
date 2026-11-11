using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    public class PosicionGrupo
    {
        [Column("equipo_id")] public int EquipoId { get; set; }
        [Column("nombre_equipo")] public string NombreEquipo { get; set; } = string.Empty;
        [Column("puntos")] public int Puntos { get; set; }
        [Column("asaltos_ganados")] public int AsaltosGanados { get; set; }
        [Column("asaltos_perdidos")] public int AsaltosPerdidos { get; set; }
        [Column("diferencia_asaltos")] public int DiferenciaAsaltos { get; set; }
    }
}

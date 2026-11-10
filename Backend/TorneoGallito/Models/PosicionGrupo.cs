using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    // Resultado de fn_tabla_posiciones(grupo)
    // TODO: ajustar nombres/tipos al RETURNS TABLE que entregue BD.
    //       Ojo: COUNT() devuelve bigint -> BD debe hacer ::int o aquí usar long.
    public class PosicionGrupo
    {
        [Column("posicion")] public int Posicion { get; set; }
        [Column("equipo_id")] public int EquipoId { get; set; }
        [Column("equipo")] public string Equipo { get; set; } = string.Empty;
        [Column("color_hex")] public string ColorHex { get; set; } = string.Empty;
        [Column("jugados")] public int Jugados { get; set; }
        [Column("ganados")] public int Ganados { get; set; }
        [Column("empatados")] public int Empatados { get; set; }
        [Column("perdidos")] public int Perdidos { get; set; }
        [Column("asaltos_ganados")] public int AsaltosGanados { get; set; }
        [Column("asaltos_perdidos")] public int AsaltosPerdidos { get; set; }
        [Column("diferencia_asaltos")] public int DiferenciaAsaltos { get; set; }
        [Column("puntos")] public int Puntos { get; set; }
    }
}

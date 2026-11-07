using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    // Resultado de fn_combate_en_curso(): combate en curso o en pausa, con marcador y asalto abierto
    // TODO: ajustar al RETURNS TABLE que entregue BD.
    public class CombateEnCurso
    {
        [Column("combate_id")] public int CombateId { get; set; }
        [Column("numero_orden")] public int NumeroOrden { get; set; }
        [Column("fase")] public string Fase { get; set; } = string.Empty;
        [Column("estado")] public string Estado { get; set; } = string.Empty;
        [Column("equipo_a_id")] public int EquipoAId { get; set; }
        [Column("equipo_a")] public string EquipoA { get; set; } = string.Empty;
        [Column("color_a")] public string ColorA { get; set; } = string.Empty;
        [Column("equipo_b_id")] public int EquipoBId { get; set; }
        [Column("equipo_b")] public string EquipoB { get; set; } = string.Empty;
        [Column("color_b")] public string ColorB { get; set; } = string.Empty;
        [Column("asaltos_a")] public int AsaltosA { get; set; }
        [Column("asaltos_b")] public int AsaltosB { get; set; }
        [Column("asalto_id")] public int? AsaltoId { get; set; }            // null si no hay asalto abierto
        [Column("asalto_numero")] public int? AsaltoNumero { get; set; }
        [Column("es_muerte_subita")] public bool? EsMuerteSubita { get; set; }
        [Column("asalto_inicio")] public DateTime? AsaltoInicio { get; set; }
    }
}

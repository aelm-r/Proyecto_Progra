using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    // Resultado de fn_siguiente_combate()
    // TODO: ajustar al RETURNS TABLE que entregue BD.
    public class SiguienteCombate
    {
        [Column("combate_id")] public int CombateId { get; set; }
        [Column("numero_orden")] public int NumeroOrden { get; set; }
        [Column("fase")] public string Fase { get; set; } = string.Empty;
        [Column("grupo")] public string? Grupo { get; set; }
        [Column("equipo_a_id")] public int EquipoAId { get; set; }
        [Column("equipo_a")] public string EquipoA { get; set; } = string.Empty;
        [Column("color_a")] public string ColorA { get; set; } = string.Empty;
        [Column("equipo_b_id")] public int EquipoBId { get; set; }
        [Column("equipo_b")] public string EquipoB { get; set; } = string.Empty;
        [Column("color_b")] public string ColorB { get; set; } = string.Empty;
    }
}

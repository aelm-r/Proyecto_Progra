using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    // Resultado de fn_bracket(): los 4 combates eliminatorios («Ganador SF1» mientras no se conoce)
    // TODO: ajustar al RETURNS TABLE que entregue BD.
    public class LlaveBracket
    {
        [Column("combate_id")] public int CombateId { get; set; }
        [Column("numero_orden")] public int NumeroOrden { get; set; }
        [Column("fase")] public string Fase { get; set; } = string.Empty;
        [Column("llave")] public string? Llave { get; set; }
        [Column("equipo_a_id")] public int? EquipoAId { get; set; }
        [Column("equipo_a")] public string EquipoA { get; set; } = string.Empty;   // nombre o «Ganador SF1»
        [Column("equipo_b_id")] public int? EquipoBId { get; set; }
        [Column("equipo_b")] public string EquipoB { get; set; } = string.Empty;
        [Column("estado")] public string Estado { get; set; } = string.Empty;
        [Column("marcador")] public string? Marcador { get; set; }
        [Column("ganador_id")] public int? GanadorId { get; set; }
    }
}

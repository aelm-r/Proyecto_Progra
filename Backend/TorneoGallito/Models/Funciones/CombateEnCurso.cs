using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    public class CombateEnCurso
    {
        [Column("combate_id")] public int CombateId { get; set; }
        [Column("numero_orden")] public int NumeroOrden { get; set; }
        [Column("estado")] public string Estado { get; set; } = string.Empty;
        [Column("equipo_a")] public string EquipoA { get; set; } = string.Empty;
        [Column("equipo_b")] public string EquipoB { get; set; } = string.Empty;
        [Column("asalto_actual")] public int? AsaltoActual { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    public class LlaveBracket
    {
        [Column("combate_id")] public int CombateId { get; set; }
        [Column("numero_orden")] public int NumeroOrden { get; set; }
        [Column("fase")] public string Fase { get; set; } = string.Empty;
        [Column("llave")] public string? Llave { get; set; }
        [Column("equipo_a")] public string EquipoA { get; set; } = string.Empty;
        [Column("equipo_b")] public string EquipoB { get; set; } = string.Empty;
        [Column("ganador")] public string Ganador { get; set; } = string.Empty;
    }
}

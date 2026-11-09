using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    // Resultado de fn_historial_equipo(equipo)
    // TODO: ajustar al RETURNS TABLE que entregue BD.
    public class HistorialEquipo
    {
        [Column("combate_id")] public int CombateId { get; set; }
        [Column("numero_orden")] public int NumeroOrden { get; set; }
        [Column("fase")] public string Fase { get; set; } = string.Empty;
        [Column("rival")] public string Rival { get; set; } = string.Empty;
        [Column("resultado")] public string Resultado { get; set; } = string.Empty; // victoria, empate, derrota
        [Column("marcador")] public string? Marcador { get; set; }
        [Column("estado")] public string Estado { get; set; } = string.Empty;
    }
}

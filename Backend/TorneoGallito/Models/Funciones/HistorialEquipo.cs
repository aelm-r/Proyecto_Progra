using System.ComponentModel.DataAnnotations.Schema;

namespace TorneoGallito.Models.Funciones
{
    public class HistorialEquipo
    {
        [Column("combate_id")] public int CombateId { get; set; }
        [Column("fase")] public string Fase { get; set; } = string.Empty;
        [Column("rival")] public string Rival { get; set; } = string.Empty;
        [Column("resultado_combate")] public string ResultadoCombate { get; set; } = string.Empty;
    }
}

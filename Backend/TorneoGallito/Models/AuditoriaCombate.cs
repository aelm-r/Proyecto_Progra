namespace TorneoGallito.Models
{
    public class AuditoriaCombate
    {
        public int Id { get; set; }
        public int CombateId { get; set; }
        public string Campo { get; set; } = string.Empty;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public int? UsuarioId { get; set; }
        public DateTime FechaCambio { get; set; }

        public Combate Combate { get; set; } = null;
        public Usuario? Usuario { get; set; }

    }
}

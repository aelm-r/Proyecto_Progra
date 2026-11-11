namespace TorneoGallito.Models
{
    
    public class Grupo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<GrupoEquipo> Equipos { get; set; } = new List<GrupoEquipo>();
    }
}

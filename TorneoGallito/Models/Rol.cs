namespace TorneoGallito.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Ej: Admin, Juez

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
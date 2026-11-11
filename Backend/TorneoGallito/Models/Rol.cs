namespace TorneoGallito.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } //  Admin, Juez

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
namespace TorneoGallito.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string NombreCompleto { get; set; }
        public int RolId { get; set; }
        public bool Activo { get; set; }

        public Rol Rol { get; set; } = null!;
    }
}
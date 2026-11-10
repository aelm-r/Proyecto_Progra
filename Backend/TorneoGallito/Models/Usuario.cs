namespace TorneoGallito.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Activo { get; set; } = true;
        public Rol Rol { get; set; } = null!;
    }
}

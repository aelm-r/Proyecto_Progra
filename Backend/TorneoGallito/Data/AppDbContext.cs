using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TorneoGallito.Models;
//using TorneoGallito.Models

namespace TorneoGallito.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        //esto le dice a EF Core que cree una tabla llamada "Estudiantes" basada en nuestro modelo
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        //public DbSet<Tabla3> Cursos { get; set; }
        //public DbSet<Tabla4> Catedraticos { get; set; }
    }
}

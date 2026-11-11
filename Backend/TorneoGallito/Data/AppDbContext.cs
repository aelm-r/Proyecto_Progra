using Microsoft.EntityFrameworkCore;
using TorneoGallito.Models;

namespace TorneoGallito.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Integrante> Integrantes { get; set; }
        public DbSet<Robot> Robots { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<GrupoEquipo> GrupoEquipos { get; set; }
        public DbSet<Combate> Combates { get; set; }
        public DbSet<Asalto> Asaltos { get; set; }
        public DbSet<Amonestacion> Amonestaciones { get; set; }
        public DbSet<PausaTecnica> PausasTecnicas { get; set; }
        public DbSet<AuditoriaCombate> AuditoriaCombates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>().HasIndex(r => r.Nombre).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Equipo>().HasIndex(e => e.Nombre).IsUnique();
            modelBuilder.Entity<Integrante>().HasIndex(i => i.Carne).IsUnique();
            modelBuilder.Entity<Grupo>().HasIndex(g => g.Nombre).IsUnique();
            modelBuilder.Entity<Combate>().HasIndex(c => c.NumeroOrden).IsUnique();
            modelBuilder.Entity<Asalto>().HasIndex(a => new { a.CombateId, a.Numero }).IsUnique();
            modelBuilder.Entity<PausaTecnica>().HasIndex(p => new { p.CombateId, p.EquipoId }).IsUnique();

            modelBuilder.Entity<Robot>()
                .HasOne(r => r.Equipo).WithOne(e => e.Robot)
                .HasForeignKey<Robot>(r => r.EquipoId);

            modelBuilder.Entity<GrupoEquipo>(e =>
            {
                e.HasKey(ge => new { ge.GrupoId, ge.EquipoId });
                e.HasIndex(ge => new { ge.GrupoId, ge.PosicionSorteo }).IsUnique();
                e.HasOne(ge => ge.Equipo).WithOne(eq => eq.GrupoEquipo)
                 .HasForeignKey<GrupoEquipo>(ge => ge.EquipoId);
            });

            
            modelBuilder.Entity<Combate>(e =>
            {
                e.HasOne(c => c.EquipoA).WithMany().HasForeignKey(c => c.EquipoAId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(c => c.EquipoB).WithMany().HasForeignKey(c => c.EquipoBId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(c => c.Ganador).WithMany().HasForeignKey(c => c.GanadorId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(c => c.UsuarioActualizo).WithMany().HasForeignKey(c => c.ActualizadoPor).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Asalto>()
                .HasOne(a => a.Ganador).WithMany().HasForeignKey(a => a.GanadorId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Amonestacion>()
                .HasOne(a => a.Equipo).WithMany().HasForeignKey(a => a.EquipoId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PausaTecnica>()
                .HasOne(p => p.Equipo).WithMany().HasForeignKey(p => p.EquipoId).OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Rol>().ToTable(t => t.HasCheckConstraint("ck_roles_nombre", "nombre IN ('Admin', 'Juez')"));
            modelBuilder.Entity<Equipo>().ToTable(t => t.HasCheckConstraint("ck_equipos_color_hex", "color_hex ~ '^#[0-9A-Fa-f]{6}$'"));
            modelBuilder.Entity<Robot>().ToTable(t => t.HasCheckConstraint("ck_robots_estado", "estado IN ('activo', 'en_reparacion', 'descalificado')"));
            modelBuilder.Entity<Grupo>().ToTable(t => t.HasCheckConstraint("ck_grupos_nombre", "nombre IN ('A', 'B')"));
            modelBuilder.Entity<GrupoEquipo>().ToTable(t => t.HasCheckConstraint("ck_grupo_equipos_posicion", "posicion_sorteo BETWEEN 1 AND 4"));
            modelBuilder.Entity<Combate>().ToTable(t =>
            {
                t.HasCheckConstraint("ck_combates_numero_orden", "numero_orden BETWEEN 1 AND 16");
                t.HasCheckConstraint("ck_combates_fase", "fase IN ('grupos', 'semifinal', 'tercer_lugar', 'final')");
                t.HasCheckConstraint("ck_combates_estado", "estado IN ('programado', 'en_curso', 'en_pausa', 'finalizado', 'cancelado')");
                t.HasCheckConstraint("ck_combates_equipos_distintos", "equipo_a_id IS NULL OR equipo_b_id IS NULL OR equipo_a_id <> equipo_b_id");
            });
            modelBuilder.Entity<Asalto>().ToTable(t => t.HasCheckConstraint("ck_asaltos_resultado",
                "resultado IS NULL OR resultado IN ('reventon', 'autogolpe', 'fuera_arena', 'inmovil', 'sancion', 'nulo', 'decision_juez')"));
        }
    }
}

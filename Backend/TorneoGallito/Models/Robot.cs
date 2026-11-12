namespace TorneoGallito.Models
{
    
    public class Robot
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string Estado { get; set; } = "activo";

        public Equipo Equipo { get; set; } = null!;

         
        public Robot(int id, int equipoId, string nombre, string? fotoUrl, string estado)
        {
            Id = id;
            EquipoId = equipoId;
            Nombre = nombre;
            FotoUrl = fotoUrl;
            Estado = estado;
        }

        public Robot()
        {

        }
    }
}

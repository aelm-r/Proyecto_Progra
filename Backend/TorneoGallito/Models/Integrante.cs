namespace TorneoGallito.Models
{
    
    public class Integrante
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Carne { get; set; } = string.Empty; 

        public Equipo Equipo { get; set; } = null!;

        
        public Integrante(int id, int equipoId, string nombreCompleto, string carne)
        {
            Id = id;
            EquipoId = equipoId;
            NombreCompleto = nombreCompleto;
            Carne = carne;
        }

        public Integrante()
        {

        }
    }
}

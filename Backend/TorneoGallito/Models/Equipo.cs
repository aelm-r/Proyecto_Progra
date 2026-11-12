namespace TorneoGallito.Models
{
    
    public class Equipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Lema { get; set; }
        public string ColorHex { get; set; } = string.Empty; 
        public string? LogoUrl { get; set; }

        public ICollection<Integrante> Integrantes { get; set; } = new List<Integrante>();
        public Robot? Robot { get; set; }              
        public GrupoEquipo? GrupoEquipo { get; set; }   

        
        public Equipo(int id, string nombre, string? lema, string colorHex, string? logoUrl)
        {
            Id = id;
            Nombre = nombre;
            Lema = lema;
            ColorHex = colorHex;
            LogoUrl = logoUrl;
        }

        public Equipo()
        {

        }
    }
}

namespace TorneoGallito.DTOs.Responses
{
    public class PosicionResponseDTO
    {
        public int Posicion { get; set; }
        public EquipoResumenDTO Equipo { get; set; } = new();
        public int Jugados { get; set; }
        public int Ganados { get; set; }
        public int Empatados { get; set; }
        public int Perdidos { get; set; }
        public int AsaltosGanados { get; set; }
        public int AsaltosPerdidos { get; set; }
        public int DiferenciaAsaltos { get; set; }
        public int Puntos { get; set; }
    }
}

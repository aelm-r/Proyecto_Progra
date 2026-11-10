namespace TorneoGallito.Constantes
{
    
    public static class TiposResultadoCombate
    {
        public const string Asaltos = "asaltos";
        public const string Empate = "empate";                 // solo en fase de grupos
        public const string WalkOver = "wo";                   // un equipo no se presentó
        public const string Descalificacion = "descalificacion";

        public static readonly string[] Todos = [Asaltos, Empate, WalkOver, Descalificacion];
    }
}

namespace TorneoGallito.Constantes
{
    public static class ResultadosAsalto
    {
        public const string Reventon = "reventon";          // gana quien reventó el globo
        public const string Autogolpe = "autogolpe";        // gana el rival
        public const string FueraArena = "fuera_arena";     // gana el rival
        public const string Inmovil = "inmovil";            // gana el rival
        public const string Sancion = "sancion";            // segunda  amonestacion del equipo: gana el rival
        public const string Nulo = "nulo";                  // nadie gana
        public const string DecisionJuez = "decision_juez"; //  muerte súbita eliminatoria

        public static readonly string[] Todos = [Reventon, Autogolpe, FueraArena, Inmovil, Sancion, Nulo, DecisionJuez];

        public const int SegundosAsalto = 90;
        public const int SegundosMuerteSubita = 60;
    }
}

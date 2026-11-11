namespace TorneoGallito.Constantes
{
    public static class TiposResultadoCombate
    {
        public const string DecisionJuez = "decision_juez";
        public const string Empate = "empate";
        public const string WalkOver = "wo";

        public static readonly string[] Todos =
        {
            ResultadosAsalto.Reventon, ResultadosAsalto.Autogolpe, ResultadosAsalto.FueraArena,
            ResultadosAsalto.Inmovil, ResultadosAsalto.Sancion, ResultadosAsalto.Nulo,
            DecisionJuez, WalkOver, Empate
        };
    }
}

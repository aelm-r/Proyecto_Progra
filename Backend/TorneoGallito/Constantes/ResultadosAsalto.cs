namespace TorneoGallito.Constantes
{
    public static class ResultadosAsalto
    {
        public const string Reventon = "reventon";          
        public const string Autogolpe = "autogolpe";        
        public const string FueraArena = "fuera_arena";     
        public const string Inmovil = "inmovil";            
        public const string Sancion = "sancion";            
        public const string Nulo = "nulo";                  
        public const string DecisionJuez = "decision_juez"; 

        public static readonly string[] Todos = { Reventon, Autogolpe, FueraArena, Inmovil, Sancion, Nulo, DecisionJuez };

        public const int SegundosAsalto = 90;
        public const int SegundosMuerteSubita = 60;
        public const int TotalAsaltos = 3;
    }
}

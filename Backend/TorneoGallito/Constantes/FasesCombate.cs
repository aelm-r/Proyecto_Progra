namespace TorneoGallito.Constantes
{
    public static class FasesCombate
    {
        public const string Grupos = "grupos";
        public const string Semifinal = "semifinal";
        public const string TercerLugar = "tercer_lugar";
        public const string Final = "final";

        public static readonly string[] Todas = [Grupos, Semifinal, TercerLugar, Final];

        // Valores q que devuelve fn_fase_evento()
        public const string EventoPreparacion = "preparacion";
        public const string EventoSorteoRealizado = "sorteo_realizado";
        public const string EventoGrupos = "grupos";
        public const string EventoEliminatoria = "eliminatoria";
        public const string EventoFinalizado = "finalizado";
    }
}

namespace TorneoGallito.Constantes
{
    public static class EstadosCombate
    {
        public const string Programado = "programado";
        public const string EnCurso = "en_curso";
        public const string EnPausa = "en_pausa";
        public const string Finalizado = "finalizado";
        public const string Cancelado = "cancelado";

        public static readonly string[] Todos = [Programado, EnCurso, EnPausa, Finalizado, Cancelado];
    }
}

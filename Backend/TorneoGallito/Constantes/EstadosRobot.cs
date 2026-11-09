namespace TorneoGallito.Constantes
{
    public static class EstadosRobot
    {
        public const string Activo = "activo";
        public const string EnReparacion = "en_reparacion";
        public const string Descalificado = "descalificado";

        public static readonly string[] Todos = [Activo, EnReparacion, Descalificado];
    }
}

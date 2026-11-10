namespace TorneoGallito.DTOs.Responses
{
    // Tabla de posiciones de un grupo (fn_tabla_posiciones)
    public class GrupoResponseDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public List<PosicionResponseDTO> Posiciones { get; set; } = new();
    }
}

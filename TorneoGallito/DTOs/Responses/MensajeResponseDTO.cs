using System.Text.Json.Serialization;

namespace TorneoGallito.DTOs.Responses
{
    // Formato para los errores 400/401/403/404/409
    public class MensajeResponseDTO
    {
        public string Mensaje { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errores { get; set; }

        public MensajeResponseDTO()
        {
        }

        public MensajeResponseDTO(string mensaje, Dictionary<string, string[]>? errores = null)
        {
            Mensaje = mensaje;
            Errores = errores;
        }
    }
}

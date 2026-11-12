using System.Text.Json.Serialization;

namespace TorneoGallito.DTOs.Responses
{
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

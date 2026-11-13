using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;

namespace TorneoGallito.Services.Interfaces
{
    public interface ICombateService
    {
        List<CombateResponseDTO> ObtenerCombates(CombateFiltroDTO filtro);
        CombateResponseDTO ObtenerCombate(int id);

        CombateResponseDTO IniciarCombate(int id, int usuarioId);
        CombateResponseDTO IniciarAsalto(int id, int usuarioId);
        CombateResponseDTO CerrarAsalto(int id, CerrarAsaltoDTO dto, int usuarioId);
        CombateResponseDTO Pausar(int id, PausaTecnicaDTO dto, int usuarioId);
        CombateResponseDTO Reanudar(int id, int usuarioId);
        CombateResponseDTO RegistrarAmonestacion(int id, AmonestacionCreateDTO dto, int usuarioId);
        CombateResponseDTO FinalizarCombate(int id, int usuarioId);
        CombateResponseDTO RegistrarWo(int id, WoDTO dto, int usuarioId);
        CombateResponseDTO RegistrarDescalificacion(int id, DescalificacionDTO dto, int usuarioId);

        CombateResponseDTO CorregirResultado(int id, CorreccionDTO dto, int usuarioId);
        CombateResponseDTO CancelarCombate(int id, CancelarCombateDTO dto, int usuarioId);
    }
}

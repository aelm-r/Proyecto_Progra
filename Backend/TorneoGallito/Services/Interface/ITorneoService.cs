using TorneoGallito.DTOs.Responses;

namespace TorneoGallito.Services.Interfaces
{
    public interface ITorneoService
    {
        SorteoResponseDTO RealizarSorteo(int usuarioId);
        List<LlaveResponseDTO> GenerarSemifinales(int usuarioId);
    }
}

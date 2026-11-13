using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;

namespace TorneoGallito.Services.Interfaces
{
    public interface IPublicService
    {
        EstadoEventoResponseDTO ObtenerEstadoEvento();
        SorteoResponseDTO ObtenerSorteo();
        List<GrupoResponseDTO> ObtenerGrupos();
        List<LlaveResponseDTO> ObtenerBracket();
        List<CombateResponseDTO> ObtenerFixture();
        List<HistorialEquipoResponseDTO> ObtenerHistorialEquipo(int equipoId);
        EstadisticasEquipoResponseDTO ObtenerEstadisticasEquipo(int equipoId);
    }
}

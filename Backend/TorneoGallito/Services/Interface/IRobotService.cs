using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;

namespace TorneoGallito.Services.Interfaces
{
    public interface IRobotService
    {
        RobotResponseDTO ActualizarRobot(int id, RobotUpdateDTO dto);
        RobotResponseDTO CambiarEstado(int id, RobotEstadoDTO dto);
    }
}

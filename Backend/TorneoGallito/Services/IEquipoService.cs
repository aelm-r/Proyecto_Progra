using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;

namespace TorneoGallito.Services.Interfaces
{
    public interface IEquipoService
    {
        List<EquipoResponseDTO> ObtenerEquipos();
        EquipoResponseDTO ObtenerEquipo(int id);
        EquipoResponseDTO CrearEquipo(EquipoCreateDTO dto);
        EquipoResponseDTO ActualizarEquipo(int id, EquipoUpdateDTO dto);
        void EliminarEquipo(int id);
        IntegranteResponseDTO AgregarIntegrante(int equipoId, IntegranteCreateDTO dto);
        RobotResponseDTO CrearRobot(int equipoId, RobotCreateDTO dto);
    }
}

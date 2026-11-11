using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoGallito.Constantes;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Controllers
{
    // Catálogos
    [Route("api/[controller]")]
    [ApiController]
    public class RobotsController : ControllerBase
    {
        private readonly IRobotService _robotService;

        public RobotsController(IRobotService robotService)
        {
            _robotService = robotService;
        }

        // PUT: api/robots/5
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<RobotResponseDTO> ActualizarRobot(int id, [FromBody] RobotUpdateDTO dto)
        {
            var respuesta = _robotService.ActualizarRobot(id, dto);
            return Ok(respuesta);
        }

        // PATCH: api/robots/5/estado el jueexz tambien puede cambiar el estado de un robot gallito
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = Roles.AdminOJuez)]
        public ActionResult<RobotResponseDTO> CambiarEstado(int id, [FromBody] RobotEstadoDTO dto)
        {
            var respuesta = _robotService.CambiarEstado(id, dto);
            return Ok(respuesta);
        }
    }
}

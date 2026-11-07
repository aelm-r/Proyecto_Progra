using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoGallito.Constantes;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Controllers
{
    // Escuadra: Catálogos
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class EquiposController : ControllerBase
    {
        private readonly IEquipoService _equipoService;

        // Inyectamos el servicio mediante el constructor
        public EquiposController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        // GET: api/equipos
        [HttpGet]
        public ActionResult<List<EquipoResponseDTO>> ObtenerEquipos()
        {
            var respuesta = _equipoService.ObtenerEquipos();
            return Ok(respuesta);
        }

        // GET: api/equipos/5
        [HttpGet("{id}")]
        public ActionResult<EquipoResponseDTO> ObtenerEquipo(int id)
        {
            var respuesta = _equipoService.ObtenerEquipo(id);
            return Ok(respuesta);
        }

        // POST: api/equipos (solo antes del sorteo)
        [HttpPost]
        public ActionResult<EquipoResponseDTO> CrearEquipo([FromBody] EquipoCreateDTO dto)
        {
            var nuevoEquipo = _equipoService.CrearEquipo(dto);
            return Created("", nuevoEquipo);
        }

        // PUT: api/equipos/5
        [HttpPut("{id}")]
        public ActionResult<EquipoResponseDTO> ActualizarEquipo(int id, [FromBody] EquipoUpdateDTO dto)
        {
            var equipoActualizado = _equipoService.ActualizarEquipo(id, dto);
            return Ok(equipoActualizado);
        }

        // DELETE: api/equipos/5 (solo antes del sorteo)
        [HttpDelete("{id}")]
        public IActionResult EliminarEquipo(int id)
        {
            _equipoService.EliminarEquipo(id);
            return Ok(new MensajeResponseDTO("Equipo eliminado correctamente"));
        }

        // POST: api/equipos/5/integrantes
        [HttpPost("{id}/integrantes")]
        public ActionResult<IntegranteResponseDTO> AgregarIntegrante(int id, [FromBody] IntegranteCreateDTO dto)
        {
            var nuevoIntegrante = _equipoService.AgregarIntegrante(id, dto);
            return Created("", nuevoIntegrante);
        }

        // POST: api/equipos/5/robot (uno por equipo)
        [HttpPost("{id}/robot")]
        public ActionResult<RobotResponseDTO> CrearRobot(int id, [FromBody] RobotCreateDTO dto)
        {
            var nuevoRobot = _equipoService.CrearRobot(id, dto);
            return Created("", nuevoRobot);
        }
    }
}

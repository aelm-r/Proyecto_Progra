using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Controllers
{
    // Escuadra: Torneo. Sin login: lo consume la pantalla TV cada 2 segundos.
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PublicController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public PublicController(IPublicService publicService)
        {
            _publicService = publicService;
        }

        // GET: api/public/estado-evento -> todo lo de la TV en una llamada, con la hora del servidor
        [HttpGet("estado-evento")]
        public ActionResult<EstadoEventoResponseDTO> ObtenerEstadoEvento()
        {
            return Ok(_publicService.ObtenerEstadoEvento());
        }

        // GET: api/public/sorteo
        [HttpGet("sorteo")]
        public ActionResult<SorteoResponseDTO> ObtenerSorteo()
        {
            return Ok(_publicService.ObtenerSorteo());
        }

        // GET: api/public/grupos
        [HttpGet("grupos")]
        public ActionResult<List<GrupoResponseDTO>> ObtenerGrupos()
        {
            return Ok(_publicService.ObtenerGrupos());
        }

        // GET: api/public/bracket
        [HttpGet("bracket")]
        public ActionResult<List<LlaveResponseDTO>> ObtenerBracket()
        {
            return Ok(_publicService.ObtenerBracket());
        }

        // GET: api/public/fixture
        [HttpGet("fixture")]
        public ActionResult<List<CombateResponseDTO>> ObtenerFixture()
        {
            return Ok(_publicService.ObtenerFixture());
        }

        // GET: api/public/equipos/5/historial
        [HttpGet("equipos/{id}/historial")]
        public ActionResult<List<HistorialEquipoResponseDTO>> ObtenerHistorialEquipo(int id)
        {
            return Ok(_publicService.ObtenerHistorialEquipo(id));
        }
    }
}

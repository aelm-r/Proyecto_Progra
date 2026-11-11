using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoGallito.Constantes;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Extensions;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Controllers
{
    //  Torneo
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class TorneoController : ControllerBase
    {
        private readonly ITorneoService _torneoService;

        public TorneoController(ITorneoService torneoService)
        {
            _torneoService = torneoService;
        }

        // POST api/torneo/sorteo - esto se hace una sola ve, y solo si hay 8 equipos.
        [HttpPost("sorteo")]
        public ActionResult<SorteoResponseDTO> RealizarSorteo()
        {
            var respuesta = _torneoService.RealizarSorteo(User.ObtenerUsuarioId());
            return Ok(respuesta);
        }

        // POST api/torneo/semifinales
        [HttpPost("semifinales")]
        public ActionResult<List<LlaveResponseDTO>> GenerarSemifinales()
        {
            var respuesta = _torneoService.GenerarSemifinales(User.ObtenerUsuarioId());
            return Ok(respuesta);
        }
    }
}

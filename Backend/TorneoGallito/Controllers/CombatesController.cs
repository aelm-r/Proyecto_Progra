using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoGallito.Constantes;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Extensions;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Controllers
{
    // Escuadra: Torneo. Dirigir combates: Juez y Admin. Corregir y cancelar
    // El id del juez se lee del token con User.ObtenerUsuarioId(), 
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.AdminOJuez)]
    public class CombatesController : ControllerBase
    {
        private readonly ICombateService _combateService;

        public CombatesController(ICombateService combateService)
        {
            _combateService = combateService;
        }

        // GET: api/combates?fase=grupos&estado=programado
        [HttpGet]
        public ActionResult<List<CombateResponseDTO>> ObtenerCombates([FromQuery] CombateFiltroDTO filtro)
        {
            return Ok(_combateService.ObtenerCombates(filtro));
        }

        // GET: api/combates/5
        [HttpGet("{id}")]
        public ActionResult<CombateResponseDTO> ObtenerCombate(int id)
        {
            return Ok(_combateService.ObtenerCombate(id));
        }

        // POST: api/combates/5/iniciar
        [HttpPost("{id}/iniciar")]
        public ActionResult<CombateResponseDTO> IniciarCombate(int id)
        {
            return Ok(_combateService.IniciarCombate(id, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/asaltos
        [HttpPost("{id}/asaltos")]
        public ActionResult<CombateResponseDTO> IniciarAsalto(int id)
        {
            return Ok(_combateService.IniciarAsalto(id, User.ObtenerUsuarioId()));
        }

        // PUT: api/combates/5/asaltos/actual
        [HttpPut("{id}/asaltos/actual")]
        public ActionResult<CombateResponseDTO> CerrarAsalto(int id, [FromBody] CerrarAsaltoDTO dto)
        {
            return Ok(_combateService.CerrarAsalto(id, dto, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/pausa
        [HttpPost("{id}/pausa")]
        public ActionResult<CombateResponseDTO> Pausar(int id, [FromBody] PausaTecnicaDTO dto)
        {
            return Ok(_combateService.Pausar(id, dto, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/reanudar
        [HttpPost("{id}/reanudar")]
        public ActionResult<CombateResponseDTO> Reanudar(int id)
        {
            return Ok(_combateService.Reanudar(id, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/amonestaciones
        [HttpPost("{id}/amonestaciones")]
        public ActionResult<CombateResponseDTO> RegistrarAmonestacion(int id, [FromBody] AmonestacionCreateDTO dto)
        {
            return Ok(_combateService.RegistrarAmonestacion(id, dto, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/finalizar
        [HttpPost("{id}/finalizar")]
        public ActionResult<CombateResponseDTO> FinalizarCombate(int id)
        {
            return Ok(_combateService.FinalizarCombate(id, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/wo
        [HttpPost("{id}/wo")]
        public ActionResult<CombateResponseDTO> RegistrarWo(int id, [FromBody] WoDTO dto)
        {
            return Ok(_combateService.RegistrarWo(id, dto, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/descalificacion
        [HttpPost("{id}/descalificacion")]
        public ActionResult<CombateResponseDTO> RegistrarDescalificacion(int id, [FromBody] DescalificacionDTO dto)
        {
            return Ok(_combateService.RegistrarDescalificacion(id, dto, User.ObtenerUsuarioId()));
        }

        // PUT: api/combates/5/correccion (solo Admin)
        [HttpPut("{id}/correccion")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<CombateResponseDTO> CorregirResultado(int id, [FromBody] CorreccionDTO dto)
        {
            return Ok(_combateService.CorregirResultado(id, dto, User.ObtenerUsuarioId()));
        }

        // POST: api/combates/5/cancelar (solo Admin)
        [HttpPost("{id}/cancelar")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<CombateResponseDTO> CancelarCombate(int id, [FromBody] CancelarCombateDTO dto)
        {
            return Ok(_combateService.CancelarCombate(id, dto, User.ObtenerUsuarioId()));
        }
    }
}

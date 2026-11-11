using Microsoft.EntityFrameworkCore;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Services
{
    public class TorneoService : ITorneoService
    {
        private readonly AppDbContext _context;
        private readonly IPublicService _publicService;

        public TorneoService(AppDbContext context, IPublicService publicService)
        {
            _context = context;
            _publicService = publicService;
        }

        public SorteoResponseDTO RealizarSorteo(int usuarioId)
        {
            _context.Database.ExecuteSqlInterpolated($"CALL sp_realizar_sorteo()");

            return _publicService.ObtenerSorteo();
        }

        public List<LlaveResponseDTO> GenerarSemifinales(int usuarioId)
        {
            _context.Database.ExecuteSqlInterpolated($"CALL sp_generar_semifinales()");

            return _publicService.ObtenerBracket();
        }
    }
}

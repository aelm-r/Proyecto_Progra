using TorneoGallito.Constantes;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Exceptions;
using TorneoGallito.Models;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Services
{
    //  Catálogos
    public class RobotService : IRobotService
    {
        private readonly AppDbContext _context;

        
        public RobotService(AppDbContext context)
        {
            _context = context;
        }

        // PUT: api/robots/5 solo Admin
        public RobotResponseDTO ActualizarRobot(int id, RobotUpdateDTO dto)
        {
            var robotDB = BuscarRobot(id);

            robotDB.Nombre = dto.Nombre.Trim();
            robotDB.FotoUrl = dto.FotoUrl;

            _context.SaveChanges(); 
            return ConvertirADTO(robotDB);
        }

        // PATCH: api/robots/5/estado
        public RobotResponseDTO CambiarEstado(int id, RobotEstadoDTO dto)
        {
            var robotDB = BuscarRobot(id);

            
            if (!EstadosRobot.Todos.Contains(dto.Estado))
            {
                throw new DatosInvalidosException("El estado debe ser: activo, en_reparacion o descalificado.");
            }

           
            if (robotDB.Estado == EstadosRobot.Descalificado && dto.Estado != EstadosRobot.Descalificado)
            {
                throw new ConflictoException("El robot está descalificado y ya no puede cambiar de estado.");
            }

            
            robotDB.Estado = dto.Estado;

            _context.SaveChanges(); 
            return ConvertirADTO(robotDB);
        }

        

        private Robot BuscarRobot(int id)
        {
            var robot = _context.Robots.FirstOrDefault(r => r.Id == id);
            if (robot == null)
            {
                throw new NoEncontradoException("Robot no encontrado");
            }
            return robot;
        }

        private static RobotResponseDTO ConvertirADTO(Robot r)
        {
            return new RobotResponseDTO
            {
                Id = r.Id,
                EquipoId = r.EquipoId,
                Nombre = r.Nombre,
                FotoUrl = r.FotoUrl,
                Estado = r.Estado
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TorneoGallito.Constantes;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Exceptions;
using TorneoGallito.Models;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Services
{
    // Escuadra: Catálogos
    public class EquipoService : IEquipoService
    {
        private readonly AppDbContext _context;

        
        public EquipoService(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/equipos
        public List<EquipoResponseDTO> ObtenerEquipos()
        {
            var lista = ConsultarEquipos()
                .OrderBy(e => e.Nombre)
                .ToList();

            return lista.Select(e => ConvertirADTO(e)).ToList();
        }

        // GET: api/equipos/5
        public EquipoResponseDTO ObtenerEquipo(int id)
        {
            var equipo = BuscarEquipo(id);
            return ConvertirADTO(equipo);
        }

        // POST: api/equipos
        public EquipoResponseDTO CrearEquipo(EquipoCreateDTO dto)
        {
            if (SorteoRealizado())
            {
                throw new ConflictoException("Solo se pueden crear equipos antes del sorteo.");
            }

            var nombre = dto.Nombre.Trim();
            if (_context.Equipos.Any(e => e.Nombre.ToLower() == nombre.ToLower()))
            {
                throw new ConflictoException($"Ya existe un equipo llamado «{nombre}».");
            }

            var nuevoEquipo = new Equipo
            {
                Nombre = nombre,
                Lema = dto.Lema,
                ColorHex = dto.ColorHex.ToUpper(),
                LogoUrl = dto.LogoUrl
            };

            if (dto.Lema != null)
            {
                nuevoEquipo.Lema = dto.Lema.Trim();
            }

            _context.Equipos.Add(nuevoEquipo);
            _context.SaveChanges(); // ejecuta el INSERT

            return ConvertirADTO(nuevoEquipo);
        }

        // PUT: api/equipos/5
        public EquipoResponseDTO ActualizarEquipo(int id, EquipoUpdateDTO dto)
        {
            var equipoDB = BuscarEquipo(id);

            // el nombre no puede repetirse con otro equipo 
            var nombre = dto.Nombre.Trim();
            if (_context.Equipos.Any(e => e.Id != id && e.Nombre.ToLower() == nombre.ToLower()))
            {
                throw new ConflictoException($"Ya existe otro equipo llamado «{nombre}».");
            }

            equipoDB.Nombre = nombre;
            equipoDB.Lema = dto.Lema;
            if (dto.Lema != null)
            {
                equipoDB.Lema = dto.Lema.Trim();
            }
            equipoDB.ColorHex = dto.ColorHex.ToUpper();
            equipoDB.LogoUrl = dto.LogoUrl;

            _context.SaveChanges(); // ejecuta el UPDATE
            return ConvertirADTO(equipoDB);
        }

        // DELETE: api/equipos/5
        public void EliminarEquipo(int id)
        {
            var equipoDB = BuscarEquipo(id);

            if (SorteoRealizado())
            {
                throw new ConflictoException("Solo se pueden eliminar equipos antes dell sorteo.");
            }

            
            _context.Equipos.Remove(equipoDB);
            _context.SaveChanges(); // ejecuta el DELETE
        }

        // POST: api/equipos/5/integrantes
        public IntegranteResponseDTO AgregarIntegrante(int equipoId, IntegranteCreateDTO dto)
        {
            if (!_context.Equipos.Any(e => e.Id == equipoId))
            {
                throw new NoEncontradoException("Equipo no encontrado");
            }

            var carne = dto.Carne.Trim();
            if (_context.Integrantes.Any(i => i.Carne == carne))
            {
                throw new ConflictoException($"Ya existe un integrante con el carné {carne}.");
            }

            var nuevoIntegrante = new Integrante
            {
                EquipoId = equipoId,
                NombreCompleto = dto.NombreCompleto.Trim(),
                Carne = carne
            };

            _context.Integrantes.Add(nuevoIntegrante);
            _context.SaveChanges(); // ejecuta el INSERT

            return new IntegranteResponseDTO
            {
                Id = nuevoIntegrante.Id,
                EquipoId = nuevoIntegrante.EquipoId,
                NombreCompleto = nuevoIntegrante.NombreCompleto,
                Carne = nuevoIntegrante.Carne
            };
        }

        // POST: api/equipos/5/robot
        public RobotResponseDTO CrearRobot(int equipoId, RobotCreateDTO dto)
        {
            if (!_context.Equipos.Any(e => e.Id == equipoId))
            {
                throw new NoEncontradoException("Equipo no encontrado");
            }
            //1 robot por equipo
            if (_context.Robots.Any(r => r.EquipoId == equipoId))
            {
                throw new ConflictoException("Este equipo ya tiene un robot registrado.");
            }

            var nuevoRobot = new Robot
            {
                EquipoId = equipoId,
                Nombre = dto.Nombre.Trim(),
                FotoUrl = dto.FotoUrl,
                Estado = EstadosRobot.Activo
            };

            _context.Robots.Add(nuevoRobot);
            _context.SaveChanges(); 

            return new RobotResponseDTO
            {
                Id = nuevoRobot.Id,
                EquipoId = nuevoRobot.EquipoId,
                Nombre = nuevoRobot.Nombre,
                FotoUrl = nuevoRobot.FotoUrl,
                Estado = nuevoRobot.Estado
            };
        }

        

        
        private IQueryable<Equipo> ConsultarEquipos()
        {
            return _context.Equipos
                .Include(e => e.Integrantes)
                .Include(e => e.Robot)
                .Include(e => e.GrupoEquipo)
                    .ThenInclude(g => g.Grupo);
        }

        private Equipo BuscarEquipo(int id)
        {
            var equipo = ConsultarEquipos().FirstOrDefault(e => e.Id == id);
            if (equipo == null)
            {
                throw new NoEncontradoException("Equipo no encontrado");
            }
            return equipo;
        }

        
        private bool SorteoRealizado()
        {
            return _context.GrupoEquipos.Any();
        }

        private static EquipoResponseDTO ConvertirADTO(Equipo e)
        {
            var dto = new EquipoResponseDTO
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Lema = e.Lema,
                ColorHex = e.ColorHex,
                LogoUrl = e.LogoUrl,
                Integrantes = e.Integrantes
                    .OrderBy(i => i.NombreCompleto)
                    .Select(i => new IntegranteResponseDTO
                    {
                        Id = i.Id,
                        EquipoId = i.EquipoId,
                        NombreCompleto = i.NombreCompleto,
                        Carne = i.Carne
                    }).ToList()
            };

            if (e.GrupoEquipo != null)
            {
                dto.Grupo = e.GrupoEquipo.Grupo.Nombre;
                dto.PosicionSorteo = e.GrupoEquipo.PosicionSorteo;
            }

            if (e.Robot != null)
            {
                dto.Robot = new RobotResponseDTO
                {
                    Id = e.Robot.Id,
                    EquipoId = e.Id,
                    Nombre = e.Robot.Nombre,
                    FotoUrl = e.Robot.FotoUrl,
                    Estado = e.Robot.Estado
                };
            }

            return dto;
        }
    }
}

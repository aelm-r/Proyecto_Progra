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

        // Inyectamos el contexto de la base de datos mediante el constructor
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
                Lema = dto.Lema?.Trim(),
                ColorHex = dto.ColorHex.ToUpper(),
                LogoUrl = dto.LogoUrl
            };

            _context.Equipos.Add(nuevoEquipo);
            _context.SaveChanges(); // ejecuta el INSERT

            return ConvertirADTO(nuevoEquipo);
        }

        // PUT: api/equipos/5
        public EquipoResponseDTO ActualizarEquipo(int id, EquipoUpdateDTO dto)
        {
            var equipoDB = BuscarEquipo(id);

            // el nombre no puede repetirse con OTRO equipo (con él mismo sí, si no lo cambia)
            var nombre = dto.Nombre.Trim();
            if (_context.Equipos.Any(e => e.Id != id && e.Nombre.ToLower() == nombre.ToLower()))
            {
                throw new ConflictoException($"Ya existe otro equipo llamado «{nombre}».");
            }

            equipoDB.Nombre = nombre;
            equipoDB.Lema = dto.Lema?.Trim();
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
                throw new ConflictoException("Solo se pueden eliminar equipos antes del sorteo.");
            }

            // como BuscarEquipo trae sus integrantes y su robot, EF los borra junto con el equipo
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

            // regla del documento: un robot por equipo
            if (_context.Robots.Any(r => r.EquipoId == equipoId))
            {
                throw new ConflictoException("Este equipo ya tiene un robot registrado. Edítalo con PUT /api/robots/{id}.");
            }

            var nuevoRobot = new Robot
            {
                EquipoId = equipoId,
                Nombre = dto.Nombre.Trim(),
                FotoUrl = dto.FotoUrl,
                Estado = EstadosRobot.Activo
            };

            _context.Robots.Add(nuevoRobot);
            _context.SaveChanges(); // ejecuta el INSERT

            return new RobotResponseDTO
            {
                Id = nuevoRobot.Id,
                EquipoId = nuevoRobot.EquipoId,
                Nombre = nuevoRobot.Nombre,
                FotoUrl = nuevoRobot.FotoUrl,
                Estado = nuevoRobot.Estado
            };
        }

        // ---------- Métodos privados de apoyo ----------

        // Equipos con todo lo que necesita el DTO: integrantes, robot y grupo del sorteo
        private IQueryable<Equipo> ConsultarEquipos()
        {
            return _context.Equipos
                .Include(e => e.Integrantes)
                .Include(e => e.Robot)
                .Include(e => e.GrupoEquipo)
                    .ThenInclude(g => g!.Grupo); // el ! es porque GrupoEquipo es null antes del sorteo
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

        // Si ya hay equipos en grupo_equipos, el sorteo ya se hizo.
        // Cuando BD entregue fn_fase_evento() se puede cambiar por:
        // _context.Database.SqlQuery<string>($"SELECT fn_fase_evento() AS \"Value\"").Single() != FasesCombate.EventoPreparacion
        private bool SorteoRealizado()
        {
            return _context.GrupoEquipos.Any();
        }

        private static EquipoResponseDTO ConvertirADTO(Equipo e)
        {
            return new EquipoResponseDTO
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Lema = e.Lema,
                ColorHex = e.ColorHex,
                LogoUrl = e.LogoUrl,
                Grupo = e.GrupoEquipo?.Grupo.Nombre,
                PosicionSorteo = e.GrupoEquipo?.PosicionSorteo,
                Robot = e.Robot == null ? null : new RobotResponseDTO
                {
                    Id = e.Robot.Id,
                    EquipoId = e.Id,
                    Nombre = e.Robot.Nombre,
                    FotoUrl = e.Robot.FotoUrl,
                    Estado = e.Robot.Estado
                },
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
        }
    }
}

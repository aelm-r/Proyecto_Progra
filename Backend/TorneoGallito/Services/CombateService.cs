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
    public class CombateService : ICombateService
    {
        private readonly AppDbContext _context;

        public CombateService(AppDbContext context)
        {
            _context = context;
        }

        public List<CombateResponseDTO> ObtenerCombates(CombateFiltroDTO filtro)
        {
            if (filtro.Fase != null && !FasesCombate.Todas.Contains(filtro.Fase))
            {
                throw new DatosInvalidosException("La fase debe ser: grupos, semifinal, tercer_lugar o final.");
            }

            if (filtro.Estado != null && !EstadosCombate.Todos.Contains(filtro.Estado))
            {
                throw new DatosInvalidosException("El estado debe ser: programado, en_curso, en_pausa, finalizado o cancelado.");
            }

            var consulta = ConsultarCombates();

            if (filtro.Fase != null)
            {
                consulta = consulta.Where(c => c.Fase == filtro.Fase);
            }

            if (filtro.Estado != null)
            {
                consulta = consulta.Where(c => c.Estado == filtro.Estado);
            }

            var combates = consulta.OrderBy(c => c.NumeroOrden).ToList();

            var respuesta = new List<CombateResponseDTO>();
            foreach (var combate in combates)
            {
                respuesta.Add(ConvertirADTO(combate));
            }
            return respuesta;
        }

        public CombateResponseDTO ObtenerCombate(int id)
        {
            var combate = ConsultarCombates().FirstOrDefault(c => c.Id == id);
            if (combate == null)
            {
                throw new NoEncontradoException("Combate no encontrado");
            }

            return ConvertirADTO(combate);
        }

        public CombateResponseDTO IniciarCombate(int id, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.Programado)
            {
                throw new ConflictoException($"El combate {combate.NumeroOrden} está «{combate.Estado}»; solo se puede iniciar si está programado.");
            }

            if (combate.EquipoAId == null || combate.EquipoBId == null)
            {
                throw new ConflictoException("El combate todavía no tiene sus dos equipos definidos.");
            }

            if (_context.Combates.Any(c => c.Estado == EstadosCombate.EnCurso || c.Estado == EstadosCombate.EnPausa))
            {
                throw new ConflictoException("Ya hay un combate en curso. Solo puede haber uno a la vez.");
            }

            combate.Estado = EstadosCombate.EnCurso;
            combate.FechaInicio = DateTime.Now;
            combate.ActualizadoPor = usuarioId;
            _context.SaveChanges();

            return ObtenerCombate(id);
        }

        public CombateResponseDTO IniciarAsalto(int id, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.EnCurso)
            {
                throw new ConflictoException("Solo se puede iniciar el asalto si el combate está en curso.");
            }

            if (BuscarAsaltoAbierto(id) != null)
            {
                throw new ConflictoException("Ya hay un asalto abierto.");
            }

            _context.Database.ExecuteSqlInterpolated($"CALL sp_iniciar_asalto({id})");

            return ObtenerCombate(id);
        }

        public CombateResponseDTO CerrarAsalto(int id, CerrarAsaltoDTO dto, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (!ResultadosAsalto.Todos.Contains(dto.Resultado))
            {
                throw new DatosInvalidosException("El resultado debe ser: reventon, autogolpe, fuera_arena, inmovil, sancion, nulo o decision_juez.");
            }

            if (dto.Resultado == ResultadosAsalto.Nulo && dto.GanadorId != null)
            {
                throw new DatosInvalidosException("Un asalto nulo no tiene ganador.");
            }

            if (dto.Resultado != ResultadosAsalto.Nulo && dto.GanadorId == null)
            {
                throw new DatosInvalidosException("Indica el equipo que ganó el asalto.");
            }

            if (dto.GanadorId != null && dto.GanadorId != combate.EquipoAId && dto.GanadorId != combate.EquipoBId)
            {
                throw new DatosInvalidosException("El ganador no pertenece a este combate.");
            }

            if (combate.Estado != EstadosCombate.EnCurso)
            {
                throw new ConflictoException("Solo se puede cerrar un asalto si el combate está en curso.");
            }

            var asalto = BuscarAsaltoAbierto(id);
            if (asalto == null)
            {
                throw new ConflictoException("No hay un asalto abierto.");
            }

            if (dto.Resultado == ResultadosAsalto.DecisionJuez && combate.Fase == FasesCombate.Grupos)
            {
                throw new ConflictoException("La decisión del juez solo se usa en eliminatorias.");
            }

            int limite = ResultadosAsalto.SegundosAsalto;
            if (asalto.EsMuerteSubita)
            {
                limite = ResultadosAsalto.SegundosMuerteSubita;
            }

            double transcurridos = (DateTime.Now - asalto.FechaInicio).TotalSeconds;
            if (dto.Resultado == ResultadosAsalto.Nulo && transcurridos < limite)
            {
                throw new ConflictoException($"Un asalto nulo por tiempo solo se acepta después de {limite} s (van {(int)transcurridos} s).");
            }

            _context.Database.ExecuteSqlInterpolated(
                $"CALL sp_cerrar_asalto({asalto.Id}, {dto.Resultado}, {dto.GanadorId}::int)");

            return ObtenerCombate(id);
        }

        public CombateResponseDTO Pausar(int id, PausaTecnicaDTO dto, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.EnCurso)
            {
                throw new ConflictoException("Solo se puede pedir pausa técnica si el combate está en curso.");
            }

            if (BuscarAsaltoAbierto(id) != null)
            {
                throw new ConflictoException("La pausa técnica es solo entre asaltos.");
            }

            if (dto.EquipoId != combate.EquipoAId && dto.EquipoId != combate.EquipoBId)
            {
                throw new DatosInvalidosException("El equipo no pertenece a este combate.");
            }

            if (_context.PausasTecnicas.Any(p => p.CombateId == id && p.EquipoId == dto.EquipoId))
            {
                throw new ConflictoException("Este equipo ya usó su pausa técnica en este combate.");
            }

            var pausa = new PausaTecnica
            {
                CombateId = id,
                EquipoId = dto.EquipoId,
                FechaInicio = DateTime.Now
            };
            _context.PausasTecnicas.Add(pausa);

            combate.Estado = EstadosCombate.EnPausa;
            combate.ActualizadoPor = usuarioId;
            _context.SaveChanges();

            return ObtenerCombate(id);
        }

        public CombateResponseDTO Reanudar(int id, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.EnPausa)
            {
                throw new ConflictoException("El combate no está en pausa.");
            }

            var pausa = _context.PausasTecnicas.FirstOrDefault(p => p.CombateId == id && p.FechaFin == null);
            if (pausa != null)
            {
                pausa.FechaFin = DateTime.Now;
            }

            combate.Estado = EstadosCombate.EnCurso;
            combate.ActualizadoPor = usuarioId;
            _context.SaveChanges();

            return ObtenerCombate(id);
        }

        public CombateResponseDTO RegistrarAmonestacion(int id, AmonestacionCreateDTO dto, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.EnCurso && combate.Estado != EstadosCombate.EnPausa)
            {
                throw new ConflictoException("Solo se puede amonestar durante un combate en curso o en pausa.");
            }

            if (dto.EquipoId != combate.EquipoAId && dto.EquipoId != combate.EquipoBId)
            {
                throw new DatosInvalidosException("El equipo no pertenece a este combate.");
            }

            var asalto = BuscarAsaltoAbierto(id);

            var amonestacion = new Amonestacion
            {
                CombateId = id,
                EquipoId = dto.EquipoId,
                Motivo = dto.Motivo.Trim(),
                Fecha = DateTime.Now
            };
            if (asalto != null)
            {
                amonestacion.AsaltoId = asalto.Id;
            }

            _context.Amonestaciones.Add(amonestacion);
            _context.SaveChanges();

            int total = _context.Amonestaciones.Count(a => a.CombateId == id && a.EquipoId == dto.EquipoId);
            if (total == 2 && asalto != null)
            {
                int rivalId = ObtenerRival(combate, dto.EquipoId);

                _context.Database.ExecuteSqlInterpolated(
                    $"CALL sp_cerrar_asalto({asalto.Id}, {ResultadosAsalto.Sancion}, {rivalId})");
            }

            return ObtenerCombate(id);
        }

        public CombateResponseDTO FinalizarCombate(int id, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.EnCurso)
            {
                throw new ConflictoException("Solo se puede finalizar un combate en curso.");
            }

            if (BuscarAsaltoAbierto(id) != null)
            {
                throw new ConflictoException("Hay un asalto abierto. Ciérrenlo antes de finalizar el combate.");
            }

            combate.ActualizadoPor = usuarioId;
            _context.SaveChanges();

            int ganadosA = _context.Asaltos.Count(a => a.CombateId == id && a.GanadorId == combate.EquipoAId);
            int ganadosB = _context.Asaltos.Count(a => a.CombateId == id && a.GanadorId == combate.EquipoBId);

            if (ganadosA == ganadosB)
            {
                if (combate.Fase != FasesCombate.Grupos)
                {
                    throw new ConflictoException("En eliminatoria no puede haber empate: el último asalto se define con decision_juez.");
                }

                combate.Estado = EstadosCombate.Finalizado;
                combate.GanadorId = null;
                combate.TipoResultado = TiposResultadoCombate.Empate;
                combate.FechaFin = DateTime.Now;
                _context.SaveChanges();

                return ObtenerCombate(id);
            }

            _context.Database.ExecuteSqlInterpolated($"CALL sp_finalizar_combate({id})");

            return ObtenerCombate(id);
        }

        public CombateResponseDTO RegistrarWo(int id, WoDTO dto, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.Programado)
            {
                throw new ConflictoException("El W.O. solo se registra antes de que empiece el combate.");
            }

            if (combate.EquipoAId == null || combate.EquipoBId == null)
            {
                throw new ConflictoException("El combate todavía no tiene sus dos equipos definidos.");
            }

            if (dto.EquipoAusenteId != combate.EquipoAId && dto.EquipoAusenteId != combate.EquipoBId)
            {
                throw new DatosInvalidosException("El equipo no pertenece a este combate.");
            }

            int ganadorId = ObtenerRival(combate, dto.EquipoAusenteId);

            combate.ActualizadoPor = usuarioId;
            _context.SaveChanges();

            _context.Database.ExecuteSqlInterpolated($"CALL sp_registrar_descalificado({id}, {dto.EquipoAusenteId})");

            PasarALaSiguienteFase(combate, ganadorId, dto.EquipoAusenteId);

            return ObtenerCombate(id);
        }

        public CombateResponseDTO RegistrarDescalificacion(int id, DescalificacionDTO dto, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado == EstadosCombate.Finalizado || combate.Estado == EstadosCombate.Cancelado)
            {
                throw new ConflictoException("No se puede descalificar en un combate finalizado o cancelado.");
            }

            if (dto.EquipoId != combate.EquipoAId && dto.EquipoId != combate.EquipoBId)
            {
                throw new DatosInvalidosException("El equipo no pertenece a este combate.");
            }

            int ganadorId = ObtenerRival(combate, dto.EquipoId);

            combate.ActualizadoPor = usuarioId;
            GuardarMotivo(id, dto.Motivo, usuarioId);
            _context.SaveChanges();

            _context.Database.ExecuteSqlInterpolated($"CALL sp_registrar_descalificado({id}, {dto.EquipoId})");

            PasarALaSiguienteFase(combate, ganadorId, dto.EquipoId);

            if (dto.DelTorneo)
            {
                var robot = _context.Robots.FirstOrDefault(r => r.EquipoId == dto.EquipoId);
                if (robot != null)
                {
                    robot.Estado = EstadosRobot.Descalificado;
                    _context.SaveChanges();
                }

                var pendientes = _context.Combates
                    .Where(c => c.Id != id && c.Estado == EstadosCombate.Programado
                             && (c.EquipoAId == dto.EquipoId || c.EquipoBId == dto.EquipoId))
                    .ToList();

                foreach (var pendiente in pendientes)
                {
                    int rivalId = ObtenerRival(pendiente, dto.EquipoId);

                    pendiente.ActualizadoPor = usuarioId;
                    GuardarMotivo(pendiente.Id, dto.Motivo, usuarioId);
                    _context.SaveChanges();

                    _context.Database.ExecuteSqlInterpolated($"CALL sp_registrar_descalificado({pendiente.Id}, {dto.EquipoId})");

                    PasarALaSiguienteFase(pendiente, rivalId, dto.EquipoId);
                }
            }

            return ObtenerCombate(id);
        }

        public CombateResponseDTO CorregirResultado(int id, CorreccionDTO dto, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.Finalizado)
            {
                throw new ConflictoException("Solo se puede corregir un combate finalizado.");
            }

            if (dto.GanadorId != null && dto.GanadorId != combate.EquipoAId && dto.GanadorId != combate.EquipoBId)
            {
                throw new DatosInvalidosException("El ganador no pertenece a este combate.");
            }

            if (dto.GanadorId == null && combate.Fase != FasesCombate.Grupos)
            {
                throw new DatosInvalidosException("Solo puede haber empate en la fase de grupos.");
            }

            if (dto.TipoResultado != null && !TiposResultadoCombate.Todos.Contains(dto.TipoResultado))
            {
                throw new DatosInvalidosException("El tipo de resultado no es válido.");
            }

            combate.GanadorId = dto.GanadorId;
            if (dto.TipoResultado != null)
            {
                combate.TipoResultado = dto.TipoResultado;
            }
            else if (dto.GanadorId == null)
            {
                combate.TipoResultado = TiposResultadoCombate.Empate;
            }

            combate.ActualizadoPor = usuarioId;
            GuardarMotivo(id, dto.Motivo, usuarioId);
            _context.SaveChanges();

            return ObtenerCombate(id);
        }

        public CombateResponseDTO CancelarCombate(int id, CancelarCombateDTO dto, int usuarioId)
        {
            var combate = BuscarCombate(id);

            if (combate.Estado != EstadosCombate.Programado)
            {
                throw new ConflictoException("Solo se puede cancelar un combate programado.");
            }

            combate.Estado = EstadosCombate.Cancelado;
            combate.ActualizadoPor = usuarioId;
            GuardarMotivo(id, dto.Motivo, usuarioId);
            _context.SaveChanges();

            return ObtenerCombate(id);
        }

        private IQueryable<Combate> ConsultarCombates()
        {
            return _context.Combates
                .AsNoTracking()
                .Include(c => c.EquipoA)
                    .ThenInclude(e => e.Robot)
                .Include(c => c.EquipoB)
                    .ThenInclude(e => e.Robot)
                .Include(c => c.Grupo)
                .Include(c => c.Asaltos);
        }

        private Combate BuscarCombate(int id)
        {
            var combate = _context.Combates.FirstOrDefault(c => c.Id == id);
            if (combate == null)
            {
                throw new NoEncontradoException("Combate no encontrado");
            }
            return combate;
        }

        private Asalto? BuscarAsaltoAbierto(int combateId)
        {
            return _context.Asaltos.FirstOrDefault(a => a.CombateId == combateId && a.FechaFin == null);
        }

        private static int ObtenerRival(Combate combate, int equipoId)
        {
            if (equipoId == combate.EquipoAId)
            {
                return (int)combate.EquipoBId;
            }
            return (int)combate.EquipoAId;
        }

        private void GuardarMotivo(int combateId, string motivo, int usuarioId)
        {
            var auditoria = new AuditoriaCombate
            {
                CombateId = combateId,
                Campo = "motivo",
                ValorNuevo = motivo.Trim(),
                UsuarioId = usuarioId,
                FechaCambio = DateTime.Now
            };
            _context.AuditoriaCombates.Add(auditoria);
        }

        private void PasarALaSiguienteFase(Combate combate, int ganadorId, int perdedorId)
        {
            if (combate.Fase != FasesCombate.Semifinal)
            {
                return;
            }

            var final = _context.Combates.FirstOrDefault(c => c.Fase == FasesCombate.Final);
            var tercerLugar = _context.Combates.FirstOrDefault(c => c.Fase == FasesCombate.TercerLugar);
            if (final == null || tercerLugar == null)
            {
                return;
            }

            if (combate.Llave == "SF1")
            {
                final.EquipoAId = ganadorId;
                tercerLugar.EquipoAId = perdedorId;
            }
            else
            {
                final.EquipoBId = ganadorId;
                tercerLugar.EquipoBId = perdedorId;
            }
            _context.SaveChanges();
        }

        private CombateResponseDTO ConvertirADTO(Combate combate)
        {
            var dto = new CombateResponseDTO
            {
                Id = combate.Id,
                NumeroOrden = combate.NumeroOrden,
                Fase = combate.Fase,
                Llave = combate.Llave,
                Estado = combate.Estado,
                EquipoA = ConvertirEquipo(combate.EquipoA),
                EquipoB = ConvertirEquipo(combate.EquipoB),
                GanadorId = combate.GanadorId,
                TipoResultado = combate.TipoResultado,
                Marcador = _context.Database
                    .SqlQuery<string>($"SELECT fn_marcador_combate({combate.Id}) AS \"Value\"")
                    .Single(),
                FechaInicio = combate.FechaInicio,
                FechaFin = combate.FechaFin
            };

            if (combate.Grupo != null)
            {
                dto.Grupo = combate.Grupo.Nombre;
            }

            foreach (var asalto in combate.Asaltos.OrderBy(a => a.Numero))
            {
                int duracion = ResultadosAsalto.SegundosAsalto;
                if (asalto.EsMuerteSubita)
                {
                    duracion = ResultadosAsalto.SegundosMuerteSubita;
                }

                dto.Asaltos.Add(new AsaltoResponseDTO
                {
                    Id = asalto.Id,
                    Numero = asalto.Numero,
                    EsMuerteSubita = asalto.EsMuerteSubita,
                    Resultado = asalto.Resultado,
                    GanadorId = asalto.GanadorId,
                    FechaInicio = asalto.FechaInicio,
                    FechaFin = asalto.FechaFin,
                    DuracionMaximaSegundos = duracion
                });
            }

            return dto;
        }

        private static EquipoResumenDTO? ConvertirEquipo(Equipo? equipo)
        {
            if (equipo == null)
            {
                return null;
            }

            var resumen = new EquipoResumenDTO
            {
                Id = equipo.Id,
                Nombre = equipo.Nombre,
                ColorHex = equipo.ColorHex,
                LogoUrl = equipo.LogoUrl
            };

            if (equipo.Robot != null)
            {
                resumen.Robot = equipo.Robot.Nombre;
            }

            return resumen;
        }
    }
}

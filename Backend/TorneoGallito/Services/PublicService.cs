using Microsoft.EntityFrameworkCore;
using TorneoGallito.Constantes;
using TorneoGallito.Data;
using TorneoGallito.DTOs.Request;
using TorneoGallito.DTOs.Responses;
using TorneoGallito.Exceptions;
using TorneoGallito.Models;
using TorneoGallito.Models.Funciones;
using TorneoGallito.Services.Interfaces;

namespace TorneoGallito.Services
{
    public class PublicService : IPublicService
    {
        private readonly AppDbContext _context;
        private readonly IEquipoService _equipoService;
        private readonly ICombateService _combateService;

        public PublicService(AppDbContext context, IEquipoService equipoService, ICombateService combateService)
        {
            _context = context;
            _equipoService = equipoService;
            _combateService = combateService;
        }

        public EstadoEventoResponseDTO ObtenerEstadoEvento()
        {
            var respuesta = new EstadoEventoResponseDTO();
            respuesta.HoraServidor = DateTime.Now;
            respuesta.Fase = _context.Database.SqlQuery<string>($"SELECT fn_fase_evento() AS \"Value\"").Single();
            respuesta.Equipos = _equipoService.ObtenerEquipos();
            respuesta.Sorteo = ObtenerSorteo();

            if (respuesta.Sorteo.Realizado)
            {
                respuesta.CombateEnCurso = ObtenerCombateEnCurso();
                respuesta.SiguienteCombate = ObtenerSiguienteCombate();
                respuesta.UltimoFinalizado = ObtenerUltimoFinalizado();
                respuesta.Grupos = ObtenerGrupos();
                respuesta.Bracket = ObtenerBracket();
            }

            if (respuesta.Fase == FasesCombate.EventoFinalizado)
            {
                respuesta.Podio = ObtenerPodio();
            }

            respuesta.PantallaActual = CalcularPantalla(respuesta);

            return respuesta;
        }

        private string CalcularPantalla(EstadoEventoResponseDTO estado)
        {
            if (estado.Fase == FasesCombate.EventoPreparacion)
            {
                return "bienvenida";
            }

            if (estado.Fase == FasesCombate.EventoSorteoRealizado)
            {
                return "sorteo";
            }

            if (estado.CombateEnCurso != null)
            {
                return "combatevivo";
            }

            if (estado.UltimoFinalizado != null && estado.UltimoFinalizado.FechaFin != null)
            {
                double segundos = (estado.HoraServidor - estado.UltimoFinalizado.FechaFin.Value).TotalSeconds;
                if (segundos >= 0 && segundos <= 15)
                {
                    return "resultado";
                }
            }

            if (estado.SiguienteCombate != null)
            {
                return "proxcombate";
            }

            if (estado.Fase == FasesCombate.EventoGrupos)
            {
                return "rotacion";
            }

            if (estado.Fase == FasesCombate.EventoEliminatoria)
            {
                return "eliminatoria";
            }

            if (estado.Fase == FasesCombate.EventoFinalizado)
            {
                return "campeon";
            }

            return "bienvenida";
        }

        public SorteoResponseDTO ObtenerSorteo()
        {
            var filas = _context.GrupoEquipos
                .Include(g => g.Grupo)
                .Include(g => g.Equipo)
                    .ThenInclude(e => e.Robot)
                .OrderBy(g => g.PosicionSorteo)
                .ThenBy(g => g.Grupo.Nombre)
                .ToList();

            var respuesta = new SorteoResponseDTO();
            respuesta.Realizado = filas.Count > 0;

            foreach (var fila in filas)
            {
                respuesta.Equipos.Add(new SorteoEquipoDTO
                {
                    Grupo = fila.Grupo.Nombre,
                    PosicionSorteo = fila.PosicionSorteo,
                    Equipo = ConvertirEquipo(fila.Equipo)
                });
            }

            return respuesta;
        }

        public List<GrupoResponseDTO> ObtenerGrupos()
        {
            var respuesta = new List<GrupoResponseDTO>();
            var grupos = _context.Grupos.OrderBy(g => g.Nombre).ToList();

            foreach (var grupo in grupos)
            {
                var filas = _context.Database
                    .SqlQuery<PosicionGrupo>($"SELECT * FROM fn_tabla_posiciones({grupo.Id})")
                    .ToList();

                var grupoDTO = new GrupoResponseDTO();
                grupoDTO.Nombre = grupo.Nombre;

                int posicion = 1;
                foreach (var fila in filas)
                {
                    var posicionDTO = new PosicionResponseDTO
                    {
                        Posicion = posicion,
                        Equipo = BuscarEquipo(fila.EquipoId),
                        AsaltosGanados = fila.AsaltosGanados,
                        AsaltosPerdidos = fila.AsaltosPerdidos,
                        DiferenciaAsaltos = fila.DiferenciaAsaltos,
                        Puntos = fila.Puntos
                    };

                    var jugados = _context.Combates
                        .Where(c => c.GrupoId == grupo.Id && c.Estado == EstadosCombate.Finalizado
                                 && (c.EquipoAId == fila.EquipoId || c.EquipoBId == fila.EquipoId))
                        .ToList();

                    posicionDTO.Jugados = jugados.Count;
                    foreach (var combate in jugados)
                    {
                        if (combate.GanadorId == fila.EquipoId)
                        {
                            posicionDTO.Ganados++;
                        }
                        else if (combate.GanadorId == null)
                        {
                            posicionDTO.Empatados++;
                        }
                        else
                        {
                            posicionDTO.Perdidos++;
                        }
                    }

                    grupoDTO.Posiciones.Add(posicionDTO);
                    posicion++;
                }

                respuesta.Add(grupoDTO);
            }

            return respuesta;
        }

        public List<LlaveResponseDTO> ObtenerBracket()
        {
            var filas = _context.Database
                .SqlQuery<LlaveBracket>($"SELECT * FROM fn_bracket()")
                .ToList();

            var respuesta = new List<LlaveResponseDTO>();

            foreach (var fila in filas)
            {
                var llave = new LlaveResponseDTO
                {
                    CombateId = fila.CombateId,
                    NumeroOrden = fila.NumeroOrden,
                    Fase = fila.Fase,
                    Llave = fila.Llave,
                    EquipoANombre = fila.EquipoA,
                    EquipoBNombre = fila.EquipoB,
                    Marcador = ObtenerMarcador(fila.CombateId)
                };

                var combate = _context.Combates.FirstOrDefault(c => c.Id == fila.CombateId);
                if (combate != null)
                {
                    llave.EquipoAId = combate.EquipoAId;
                    llave.EquipoBId = combate.EquipoBId;
                    llave.Estado = combate.Estado;
                    llave.GanadorId = combate.GanadorId;

                    if (combate.EquipoAId != null)
                    {
                        llave.EquipoA = BuscarEquipo(combate.EquipoAId.Value);
                    }
                    if (combate.EquipoBId != null)
                    {
                        llave.EquipoB = BuscarEquipo(combate.EquipoBId.Value);
                    }
                }

                respuesta.Add(llave);
            }

            return respuesta;
        }

        public List<CombateResponseDTO> ObtenerFixture()
        {
            return _combateService.ObtenerCombates(new CombateFiltroDTO());
        }

        public List<HistorialEquipoResponseDTO> ObtenerHistorialEquipo(int equipoId)
        {
            ValidarQueExisteEquipo(equipoId);

            var filas = _context.Database
                .SqlQuery<HistorialEquipo>($"SELECT * FROM fn_historial_equipo({equipoId})")
                .ToList();

            var respuesta = new List<HistorialEquipoResponseDTO>();

            foreach (var fila in filas)
            {
                var historial = new HistorialEquipoResponseDTO
                {
                    CombateId = fila.CombateId,
                    Fase = fila.Fase,
                    Rival = fila.Rival,
                    Resultado = fila.ResultadoCombate,
                    Marcador = ObtenerMarcador(fila.CombateId)
                };

                var combate = _context.Combates.FirstOrDefault(c => c.Id == fila.CombateId);
                if (combate != null)
                {
                    historial.NumeroOrden = combate.NumeroOrden;
                    historial.Estado = combate.Estado;
                }

                respuesta.Add(historial);
            }

            return respuesta;
        }

        public EstadisticasEquipoResponseDTO ObtenerEstadisticasEquipo(int equipoId)
        {
            ValidarQueExisteEquipo(equipoId);

            decimal promedio = _context.Database
                .SqlQuery<decimal>($"SELECT fn_tiempo_promedio_reventon({equipoId}) AS \"Value\"")
                .Single();

            return new EstadisticasEquipoResponseDTO
            {
                Equipo = BuscarEquipo(equipoId),
                TiempoPromedioReventonSegundos = promedio
            };
        }

        private CombateEnVivoResponseDTO? ObtenerCombateEnCurso()
        {
            var fila = _context.Database
                .SqlQuery<CombateEnCurso>($"SELECT * FROM fn_combate_en_curso()")
                .ToList()
                .FirstOrDefault();

            if (fila == null)
            {
                return null;
            }

            var combate = _combateService.ObtenerCombate(fila.CombateId);

            var enVivo = new CombateEnVivoResponseDTO
            {
                CombateId = combate.Id,
                NumeroOrden = combate.NumeroOrden,
                Fase = combate.Fase,
                FaseNombre = FasesCombate.NombreLegible(combate.Fase),
                Estado = combate.Estado,
                Marcador = combate.Marcador,
                TotalAsaltos = ResultadosAsalto.TotalAsaltos
            };

            if (combate.EquipoA != null)
            {
                enVivo.EquipoA = combate.EquipoA;
            }
            if (combate.EquipoB != null)
            {
                enVivo.EquipoB = combate.EquipoB;
            }

            foreach (var asalto in combate.Asaltos)
            {
                if (asalto.GanadorId != null && asalto.GanadorId == enVivo.EquipoA.Id)
                {
                    enVivo.AsaltosA++;
                }
                else if (asalto.GanadorId != null && asalto.GanadorId == enVivo.EquipoB.Id)
                {
                    enVivo.AsaltosB++;
                }

                if (asalto.FechaFin == null)
                {
                    enVivo.AsaltoActual = asalto;
                }
            }

            if (enVivo.AsaltoActual != null)
            {
                double transcurridos = (DateTime.Now - enVivo.AsaltoActual.FechaInicio).TotalSeconds;
                int restantes = enVivo.AsaltoActual.DuracionMaximaSegundos - (int)transcurridos;

                if (restantes < 0)
                {
                    restantes = 0;
                }

                enVivo.TiempoRestanteSegundos = restantes;
            }

            if (combate.Estado == EstadosCombate.EnPausa)
            {
                var pausa = _context.PausasTecnicas
                    .FirstOrDefault(p => p.CombateId == combate.Id && p.FechaFin == null);

                if (pausa != null)
                {
                    enVivo.PausaEquipoId = pausa.EquipoId;
                    enVivo.PausaInicio = pausa.FechaInicio;
                }
            }

            return enVivo;
        }

        private CombateResponseDTO? ObtenerSiguienteCombate()
        {
            var fila = _context.Database
                .SqlQuery<SiguienteCombate>($"SELECT * FROM fn_siguiente_combate()")
                .ToList()
                .FirstOrDefault();

            if (fila == null)
            {
                return null;
            }

            return _combateService.ObtenerCombate(fila.CombateId);
        }

        private CombateResponseDTO? ObtenerUltimoFinalizado()
        {
            var combate = _context.Combates
                .Where(c => c.Estado == EstadosCombate.Finalizado && c.FechaFin != null)
                .OrderByDescending(c => c.FechaFin)
                .FirstOrDefault();

            if (combate == null)
            {
                return null;
            }

            return _combateService.ObtenerCombate(combate.Id);
        }

        private PodioResponseDTO ObtenerPodio()
        {
            var podio = new PodioResponseDTO();

            var final = _context.Combates.FirstOrDefault(c => c.Fase == FasesCombate.Final);
            if (final != null && final.GanadorId != null)
            {
                podio.Primero = BuscarEquipo(final.GanadorId.Value);

                if (final.GanadorId == final.EquipoAId && final.EquipoBId != null)
                {
                    podio.Segundo = BuscarEquipo(final.EquipoBId.Value);
                }
                else if (final.EquipoAId != null)
                {
                    podio.Segundo = BuscarEquipo(final.EquipoAId.Value);
                }
            }

            var tercerLugar = _context.Combates.FirstOrDefault(c => c.Fase == FasesCombate.TercerLugar);
            if (tercerLugar != null && tercerLugar.GanadorId != null)
            {
                podio.Tercero = BuscarEquipo(tercerLugar.GanadorId.Value);
            }

            return podio;
        }

        private string ObtenerMarcador(int combateId)
        {
            return _context.Database
                .SqlQuery<string>($"SELECT fn_marcador_combate({combateId}) AS \"Value\"")
                .Single();
        }

        private void ValidarQueExisteEquipo(int equipoId)
        {
            if (!_context.Equipos.Any(e => e.Id == equipoId))
            {
                throw new NoEncontradoException("Equipo no encontrado");
            }
        }

        private EquipoResumenDTO BuscarEquipo(int equipoId)
        {
            var equipo = _context.Equipos
                .Include(e => e.Robot)
                .FirstOrDefault(e => e.Id == equipoId);

            if (equipo == null)
            {
                return new EquipoResumenDTO { Id = equipoId };
            }

            return ConvertirEquipo(equipo);
        }

        private static EquipoResumenDTO ConvertirEquipo(Equipo equipo)
        {
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

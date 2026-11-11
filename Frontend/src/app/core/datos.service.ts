import { Injectable } from '@angular/core';
import { Observable, delay, of, throwError } from 'rxjs';
import {
  CombateResponse, CorreccionRequest, EquipoRequest, EquipoResponse,
  EstadoRobot, GrupoResponse, IntegranteRequest
} from './models';

const falla = (status: number, mensaje: string): Observable<never> =>
  throwError(() => ({ status, error: { mensaje } }));
const respuesta = <T>(valor: T): Observable<T> => of(structuredClone(valor)).pipe(delay(200));

const ORDEN: [string, number, number][] = [
  ['A', 1, 2], ['B', 1, 2], ['A', 3, 4], ['B', 3, 4], ['A', 1, 3], ['B', 1, 3],
  ['A', 2, 4], ['B', 2, 4], ['A', 1, 4], ['B', 1, 4], ['A', 2, 3], ['B', 2, 3]
];

/**
 * SERVICIO SIMULADO: imita lo que responderá la API (mismos DTOs y mismos códigos de error).
 * Día 6: reemplazar el interior de cada método por this.http.get/post/put/delete('/api/...').
 * Las firmas (entradas y salidas) NO cambian, así que los componentes no se tocan.
 */
@Injectable({ providedIn: 'root' })
export class DatosService {
  private id = 100;
  private realizado = false;
  private grupos: GrupoResponse[] = [];
  private combates: CombateResponse[] = [];
    private equipos: EquipoResponse[] = [
    ['Titanes', '#E53935'], ['Voltios', '#1E88E5'],
    ['Cóndores', '#8E24AA'], ['Faraday', '#00897B'],
    ['Ohmios', '#F4511E'], ['Relámpago', '#FDD835'],
    ['Amperios', '#43A047'], ['Circuitos Rojos', '#6D4C41']
  ].map(([nombre, colorHex], i) => ({
    id: i + 1, nombre, colorHex, integrantes: [],
    // El equipo 8 queda sin robot a propósito: así se prueba el rechazo del sorteo.
    robot: i < 7 ? { id: i + 1, nombre: `Robot ${nombre}`, estado: 'activo' as EstadoRobot } : null
  }));

  estadoSorteo(): Observable<{ realizado: boolean }> { return respuesta({ realizado: this.realizado }); }
  listarEquipos(): Observable<EquipoResponse[]> { return respuesta(this.equipos); }
  listarGrupos(): Observable<GrupoResponse[]> { return respuesta(this.grupos); }
  listarCombates(): Observable<CombateResponse[]> { return respuesta(this.combates); }

  crearEquipo(dto: EquipoRequest): Observable<EquipoResponse> {
    if (this.realizado) return falla(409, 'No se pueden crear equipos después del sorteo.');
    if (!dto.nombre.trim()) return falla(400, 'El nombre es obligatorio.');
    if (!/^#[0-9A-Fa-f]{6}$/.test(dto.colorHex)) return falla(400, 'El color debe tener el formato #RRGGBB.');
    if (this.equipos.some(e => e.nombre.toLowerCase() === dto.nombre.trim().toLowerCase()))
      return falla(409, 'Ya existe un equipo con ese nombre.');
    const nuevo: EquipoResponse = {
      id: this.id++, nombre: dto.nombre.trim(), lema: dto.lema?.trim() || '', 
      colorHex: dto.colorHex, integrantes: [], robot: null
    };
    this.equipos.push(nuevo);
    return respuesta(nuevo);
  }

    actualizarEquipo(id: number, dto: EquipoRequest): Observable<EquipoResponse> {
    const equipo = this.equipos.find(e => e.id === id);
    if (!equipo) return falla(404, 'El equipo no existe.');
    if (!dto.nombre.trim()) return falla(400, 'El nombre es obligatorio.');
    if (!/^#[0-9A-Fa-f]{6}$/.test(dto.colorHex)) return falla(400, 'El color debe tener el formato #RRGGBB.');
    if (this.equipos.some(e => e.id !== id && e.nombre.toLowerCase() === dto.nombre.trim().toLowerCase()))
      return falla(409, 'Ya existe un equipo con ese nombre.');
    const anterior = equipo.nombre;
    const nuevo = dto.nombre.trim();
    // Si ya hubo sorteo, el nombre también se actualiza en grupos y combates.
    this.grupos.forEach(g => g.equipos.forEach(x => {
      if (x.equipoId === id) { x.nombre = nuevo; x.colorHex = dto.colorHex; }
    }));
    this.combates.forEach(c => {
      if (c.equipoA === anterior) c.equipoA = nuevo;
      if (c.equipoB === anterior) c.equipoB = nuevo;
      if (c.ganador === anterior) c.ganador = nuevo;
    });
    equipo.nombre = nuevo; equipo.lema = dto.lema?.trim() || ''; equipo.colorHex = dto.colorHex;
    return respuesta(equipo);
  }

  eliminarEquipo(id: number): Observable<null> {
    if (this.realizado) return falla(409, 'No se pueden eliminar equipos después del sorteo.');
    if (!this.equipos.some(e => e.id === id)) return falla(404, 'El equipo no existe.');
    this.equipos = this.equipos.filter(e => e.id !== id);
    return respuesta(null);
  }

  agregarIntegrante(equipoId: number, dto: IntegranteRequest): Observable<EquipoResponse> {
    const equipo = this.equipos.find(e => e.id === equipoId);
    if (!equipo) return falla(404, 'El equipo no existe.');
    if (!dto.nombreCompleto.trim() || !dto.carne.trim()) return falla(400, 'Nombre y carné son obligatorios.');
    if (this.equipos.some(e => e.integrantes.some(i => i.carne === dto.carne.trim())))
      return falla(409, 'Ese carné ya está registrado.');
    equipo.integrantes.push({ id: this.id++, nombreCompleto: dto.nombreCompleto.trim(), carne: dto.carne.trim() });
    return respuesta(equipo);
  }

  guardarRobot(equipoId: number, nombre: string): Observable<EquipoResponse> {
    const equipo = this.equipos.find(e => e.id === equipoId);
    if (!equipo) return falla(404, 'El equipo no existe.');
    if (!nombre.trim()) return falla(400, 'El nombre del robot es obligatorio.');
    if (equipo.robot) equipo.robot.nombre = nombre.trim();
    else equipo.robot = { id: this.id++, nombre: nombre.trim(), estado: 'activo' };
    return respuesta(equipo);
  }

  cambiarEstadoRobot(robotId: number, estado: EstadoRobot): Observable<null> {
    const robot = this.equipos.map(e => e.robot).find(r => r?.id === robotId);
    if (!robot) return falla(404, 'El robot no existe.');
    robot.estado = estado;
    return respuesta(null);
  }

  realizarSorteo(): Observable<GrupoResponse[]> {
    if (this.realizado) return falla(409, 'El sorteo ya se realizó.');
    if (this.equipos.length !== 8 || this.equipos.some(e => !e.robot))
      return falla(409, 'Se necesitan exactamente 8 equipos, todos con robot.');
    const mezclados = [...this.equipos].sort(() => Math.random() - 0.5);
    this.grupos = ['A', 'B'].map((nombre, g) => ({
      nombre,
      equipos: mezclados.slice(g * 4, g * 4 + 4).map((e, i) => ({
        posicion: `${nombre}${i + 1}`, equipoId: e.id, nombre: e.nombre, colorHex: e.colorHex
      }))
    }));
    const quien = (p: string) => this.grupos.flatMap(g => g.equipos).find(x => x.posicion === p)!.nombre;
    this.combates = ORDEN.map(([g, a, b], i) => ({
      id: i + 1, numeroOrden: i + 1, fase: 'grupos',
      equipoA: quien(`${g}${a}`), equipoB: quien(`${g}${b}`),
      estado: 'programado', marcador: null, ganador: null
    }));
    ['semifinal', 'semifinal', 'tercer_lugar', 'final'].forEach((fase, i) =>
      this.combates.push({ id: 13 + i, numeroOrden: 13 + i, fase, equipoA: null, equipoB: null,
        estado: 'programado', marcador: null, ganador: null }));
    this.realizado = true;
    return respuesta(this.grupos);
  }

  generarSemifinales(): Observable<CombateResponse[]> {
    if (!this.realizado) return falla(409, 'Primero se debe realizar el sorteo.');
    if (this.combates.filter(c => c.fase === 'grupos').some(c => c.estado !== 'finalizado'))
      return falla(409, 'Falta un combate de grupo por finalizar.');
    // El servidor real usa fn_tabla_posiciones; aquí solo se simula un cruce.
    const sf = this.combates.filter(c => c.fase === 'semifinal');
    sf[0].equipoA = this.grupos[0].equipos[0].nombre; sf[0].equipoB = this.grupos[1].equipos[1].nombre;
    sf[1].equipoA = this.grupos[1].equipos[0].nombre; sf[1].equipoB = this.grupos[0].equipos[1].nombre;
    return respuesta(this.combates);
  }

  corregirResultado(id: number, dto: CorreccionRequest): Observable<CombateResponse> {
    const c = this.combates.find(x => x.id === id);
    if (!c) return falla(404, 'El combate no existe.');
    if (c.estado !== 'finalizado') return falla(409, 'Solo se corrigen combates finalizados.');
    const motivo = dto.motivo.trim();
    if (motivo.length < 10 || motivo.length > 300) return falla(400, 'El motivo debe tener entre 10 y 300 caracteres.');
    const [a, b] = dto.marcador.split('–').map(n => parseInt(n, 10));
    if (isNaN(a) || isNaN(b)) return falla(400, 'Marcador inválido.');
    c.marcador = dto.marcador;
    c.ganador = a > b ? c.equipoA : c.equipoB; // el servidor decide el ganador
    return respuesta(c);
  }

  /** SOLO PRUEBAS (Hito 2: torneo simulado): finaliza los 12 combates de grupos. */
  simularFaseDeGrupos(): Observable<CombateResponse[]> {
    if (!this.realizado) return falla(409, 'Primero se debe realizar el sorteo.');
    const marcadores = ['2 – 0', '2 – 1', '1 – 2', '0 – 2'];
    this.combates.filter(c => c.fase === 'grupos').forEach((c, i) => {
      c.estado = 'finalizado';
      c.marcador = marcadores[i % 4];
      c.ganador = i % 4 < 2 ? c.equipoA : c.equipoB;
    });
    return respuesta(this.combates);
  }
}

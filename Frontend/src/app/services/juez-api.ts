// API del panel del juez: login y dirección de combates.
// Va aparte de torneo-api.ts (que es de la TV) para no tocar lo de los demás.
// El host se toma de la dirección con la que se abrió la página: en la laptop es "localhost" y en la
// tablet es la IP de la laptop (ej. 192.168.1.50). Así el mismo código sirve para las dos.
// El puerto 5256 es el mismo que usa la TV en torneo-api.ts.
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, catchError, delay, map, of, throwError } from 'rxjs';
import { COMBATES_MOCK } from './juez-mocks';
import { API_URL } from '../core/config';
// true = datos simulados (usuario juez / juez123). false = API real.
const USAR_MOCKS = false;

export type EstadoCombate = 'programado' | 'en_curso' | 'en_pausa' | 'finalizado' | 'cancelado';

export type ResultadoAsalto =
  | 'reventon' | 'autogolpe' | 'fuera_arena' | 'inmovil'
  | 'sancion' | 'nulo' | 'decision_juez';

export interface EquipoResumen {
  id: number;
  nombre: string;
  colorHex: string;
  logoUrl?: string | null;
  robot?: string | null;
}

// ---- Lo que devuelve la API (CombateResponseDTO / AsaltoResponseDTO) ----
export interface AsaltoApi {
  id: number;
  numero: number;
  esMuerteSubita: boolean;
  resultado: ResultadoAsalto | null;
  ganadorId: number | null;
  fechaInicio: string;
  fechaFin: string | null;
  duracionMaximaSegundos: number;
}

export interface CombateApi {
  id: number;
  numeroOrden: number;
  fase: string; // grupos | semifinal | tercer_lugar | final
  llave: string | null;
  grupo: string | null;
  estado: EstadoCombate;
  equipoA: EquipoResumen | null;
  equipoB: EquipoResumen | null;
  ganadorId: number | null;
  tipoResultado: string | null;
  marcador: string; // lo calcula el servidor, ej. "2 – 1"
  fechaInicio: string | null;
  fechaFin: string | null;
  asaltos: AsaltoApi[];
}

// ---- Lo que usa la pantalla ----
export interface CombateResponse {
  id: number;
  numeroOrden: number;
  fase: string;
  estado: EstadoCombate;
  equipoA: EquipoResumen | null;
  equipoB: EquipoResumen | null;
  marcador: string;
  asaltoAbierto: {
    numero: number;
    esMuerteSubita: boolean;
    fechaInicio?: string;
    duracionMaximaSegundos?: number;
  } | null;
}

/** El asalto abierto es el que todavía no tiene fechaFin. No se calcula nada más aquí. */
export function adaptarCombate(c: CombateApi): CombateResponse {
  const abierto = c.asaltos?.find((a) => !a.fechaFin) ?? null;
  return {
    id: c.id,
    numeroOrden: c.numeroOrden,
    fase: c.fase,
    estado: c.estado,
    equipoA: c.equipoA,
    equipoB: c.equipoB,
    marcador: c.marcador,
    asaltoAbierto: abierto && {
      numero: abierto.numero,
      esMuerteSubita: abierto.esMuerteSubita,
      fechaInicio: abierto.fechaInicio,
      duracionMaximaSegundos: abierto.duracionMaximaSegundos,
    },
  };
}

// Sin id del juez, fechas ni segundos: el backend los toma del token y del reloj del servidor.
export interface CerrarAsaltoRequest {
  resultado: ResultadoAsalto;
  ganadorId?: number;
}

// ---- Login (LoginResponseDTO) ----
export interface LoginResponse {
  token: string;
  expiracion: string;
  username: string;
  nombreCompleto: string;
  rol: string;
}

export interface Sesion {
  token: string;
  username: string;
  nombreCompleto: string;
  rol: string;
}

const CLAVE_SESION = 'juez_sesion';

@Injectable({ providedIn: 'root' })
export class JuezApi {
  private http = inject(HttpClient);
  private api = API_URL;
  private mock = USAR_MOCKS ? new JuezMock() : null;

  readonly usaMocks = USAR_MOCKS;
  private _sesion = signal<Sesion | null>(this.leerSesion());
  readonly sesion = this._sesion.asReadonly();

  // ---------- Sesión ----------
  login(username: string, password: string): Observable<Sesion> {
    const peticion$: Observable<LoginResponse> = this.mock
      ? this.mock.login(username, password)
      : this.http.post<LoginResponse>(`${this.api}/auth/login`, { username, password });

    return peticion$.pipe(
      map((r) => {
        if (r.rol !== 'Juez' && r.rol !== 'Admin') {
          throw new HttpErrorResponse({ status: 403, error: { mensaje: 'Este panel es solo para el juez.' } });
        }
        const s: Sesion = { token: r.token, username: r.username, nombreCompleto: r.nombreCompleto, rol: r.rol };
        try { sessionStorage.setItem(CLAVE_SESION, JSON.stringify(s)); } catch { /* sin storage */ }
        this._sesion.set(s);
        return s;
      }),
    );
  }

  salir(): void {
    try { sessionStorage.removeItem(CLAVE_SESION); } catch { /* sin storage */ }
    this._sesion.set(null);
  }

  private leerSesion(): Sesion | null {
    try {
      const t = sessionStorage.getItem(CLAVE_SESION);
      return t ? (JSON.parse(t) as Sesion) : null;
    } catch { return null; }
  }

  // ---------- Combates ----------
  listar(fase?: string, estado?: string): Observable<CombateResponse[]> {
    if (this.mock) return this.mock.listar();
    let params = new HttpParams();
    if (fase) params = params.set('fase', fase);
    if (estado) params = params.set('estado', estado);
    return this.revisar401(
      this.http.get<CombateApi[]>(`${this.api}/combates`, { params, ...this.cabeceras() })
        .pipe(map((lista) => lista.map(adaptarCombate))),
    );
  }

  iniciar(id: number)       { return this.post(id, 'iniciar'); }
  iniciarAsalto(id: number) { return this.post(id, 'asaltos'); }
  reanudar(id: number)      { return this.post(id, 'reanudar'); }
  finalizar(id: number)     { return this.post(id, 'finalizar'); }

  /** El backend exige el equipo que pide la pausa. */
  pausar(id: number, equipoId: number) { return this.post(id, 'pausa', { equipoId }); }
  /** El backend espera "equipoAusenteId". */
  wo(id: number, equipoAusenteId: number) { return this.post(id, 'wo', { equipoAusenteId }); }
  amonestar(id: number, equipoId: number, motivo: string) {
    return this.post(id, 'amonestaciones', { equipoId, motivo });
  }

  cerrarAsalto(id: number, body: CerrarAsaltoRequest): Observable<unknown> {
    if (this.mock) return this.mock.cerrarAsalto(id, body);
    return this.revisar401(
      this.http.put<unknown>(`${this.api}/combates/${id}/asaltos/actual`, body, this.cabeceras()),
    );
  }

  // ---------- Internos ----------
  private post(id: number, ruta: string, body: object = {}): Observable<unknown> {
    if (this.mock) return this.mock.accion(id, ruta, body);
    return this.revisar401(this.http.post<unknown>(`${this.api}/combates/${id}/${ruta}`, body, this.cabeceras()));
  }

  private cabeceras() {
    const t = this._sesion()?.token;
    return t ? { headers: new HttpHeaders({ Authorization: `Bearer ${t}` }) } : {};
  }

  /** Un 401 en una acción significa token vencido o inválido: se cierra la sesión local. */
  private revisar401<T>(o: Observable<T>): Observable<T> {
    return o.pipe(catchError((e: HttpErrorResponse) => {
      if (e.status === 401) this.salir();
      return throwError(() => e);
    }));
  }
}

/** Simulación local para ensayar sin backend (usuario juez / juez123). */
class JuezMock {
  private bd: CombateResponse[] = structuredClone(COMBATES_MOCK);
  private ganados = new Map<number, [number, number]>();
  private jugados = new Map<number, number>();

  constructor() {
    for (const c of this.bd) {
      const [a, b] = c.marcador.split('–').map((s) => Number(s.trim()));
      this.ganados.set(c.id, [a, b]);
      this.jugados.set(c.id, a + b);
    }
  }

  login(username: string, password: string): Observable<LoginResponse> {
    if (username === 'juez' && password === 'juez123') {
      return of({ token: 'simulado', expiracion: '', username, nombreCompleto: 'Juez de prueba', rol: 'Juez' })
        .pipe(delay(200));
    }
    return throwError(() => new HttpErrorResponse({ status: 401, error: { mensaje: 'Usuario o contraseña incorrectos.' } }));
  }

  listar() { return this.ok(structuredClone(this.bd)); }

  accion(id: number, ruta: string, body: { equipoId?: number; equipoAusenteId?: number }): Observable<unknown> {
    const c = this.bd.find((x) => x.id === id);
    if (!c) return this.fallo(404, 'El combate no existe.');
    const n = this.jugados.get(id) ?? 0;
    const [ga, gb] = this.ganados.get(id) ?? [0, 0];

    switch (ruta) {
      case 'iniciar':
        if (c.estado !== 'programado') return this.fallo(409, 'El combate no está programado.');
        if (!c.equipoA || !c.equipoB) return this.fallo(409, 'Todavía no se conocen los dos equipos.');
        if (this.bd.some((x) => x.estado === 'en_curso' || x.estado === 'en_pausa'))
          return this.fallo(409, 'Ya hay otro combate en curso.');
        c.estado = 'en_curso';
        break;
      case 'asaltos': {
        if (c.estado !== 'en_curso' || c.asaltoAbierto)
          return this.fallo(409, 'Solo se abre un asalto en un combate en curso y sin asalto abierto.');
        const sub = c.fase !== 'grupos' && n >= 3;
        c.asaltoAbierto = {
          numero: n + 1, esMuerteSubita: sub,
          fechaInicio: new Date().toISOString(), duracionMaximaSegundos: sub ? 60 : 90,
        };
        break;
      }
      case 'pausa':
        if (c.estado !== 'en_curso' || c.asaltoAbierto)
          return this.fallo(409, 'La pausa técnica es solo entre asaltos.');
        if (body.equipoId !== c.equipoA?.id && body.equipoId !== c.equipoB?.id)
          return this.fallo(400, 'El equipo no pertenece a este combate.');
        c.estado = 'en_pausa';
        break;
      case 'reanudar':
        if (c.estado !== 'en_pausa') return this.fallo(409, 'El combate no está en pausa.');
        c.estado = 'en_curso';
        break;
      case 'amonestaciones':
        if (c.estado !== 'en_curso') return this.fallo(409, 'Solo se amonesta en un combate en curso.');
        break;
      case 'finalizar':
        if (c.estado !== 'en_curso' || c.asaltoAbierto)
          return this.fallo(409, 'Cierra el asalto abierto antes de finalizar.');
        if (ga < 2 && gb < 2 && n < 3)
          return this.fallo(409, 'El combate todavía no tiene un resultado definido.');
        c.estado = 'finalizado';
        break;
      case 'wo':
        if (c.estado !== 'programado') return this.fallo(409, 'El W.O. solo aplica a un combate programado.');
        if (body.equipoAusenteId !== c.equipoA?.id && body.equipoAusenteId !== c.equipoB?.id)
          return this.fallo(400, 'El equipo no pertenece al combate.');
        c.marcador = body.equipoAusenteId === c.equipoA?.id ? '0 – 2' : '2 – 0';
        c.estado = 'finalizado';
        break;
      default:
        return this.fallo(404, 'Acción no encontrada.');
    }
    return this.ok(structuredClone(c));
  }

  cerrarAsalto(id: number, body: CerrarAsaltoRequest): Observable<unknown> {
    const c = this.bd.find((x) => x.id === id);
    if (!c) return this.fallo(404, 'El combate no existe.');
    if (!c.asaltoAbierto) return this.fallo(409, 'No hay un asalto abierto.');
    const g = this.ganados.get(id) ?? [0, 0];
    if (body.resultado !== 'nulo') {
      if (body.ganadorId === c.equipoA?.id) g[0]++;
      else if (body.ganadorId === c.equipoB?.id) g[1]++;
      else return this.fallo(400, 'El ganador no pertenece a este combate.');
    }
    this.ganados.set(id, g);
    this.jugados.set(id, (this.jugados.get(id) ?? 0) + 1);
    c.marcador = `${g[0]} – ${g[1]}`;
    c.asaltoAbierto = null;
    return this.ok(structuredClone(c));
  }

  private ok<T>(v: T) { return of(v).pipe(delay(150)); }
  private fallo(status: number, mensaje: string) {
    return throwError(() => new HttpErrorResponse({ status, error: { mensaje } }));
  }
}

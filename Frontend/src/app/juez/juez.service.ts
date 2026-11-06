import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, delay, of, throwError } from 'rxjs';
import { JUEZ_CONFIG as environment } from './juez.config';
import { COMBATES_MOCK } from './juez.mocks';
import { CerrarAsaltoRequest, CombateResponse } from './juez.models';

@Injectable({ providedIn: 'root' })
export class JuezService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/combates`;
  private mock = environment.useMocks ? new JuezMock() : null;

  listar(fase?: string, estado?: string): Observable<CombateResponse[]> {
    if (this.mock) return this.mock.listar();
    let params = new HttpParams();
    if (fase) params = params.set('fase', fase);
    if (estado) params = params.set('estado', estado);
    return this.http.get<CombateResponse[]>(this.base, { params });
  }

  iniciar(id: number)       { return this.post(id, 'iniciar'); }
  iniciarAsalto(id: number) { return this.post(id, 'asaltos'); }
  pausar(id: number)        { return this.post(id, 'pausa'); }
  reanudar(id: number)      { return this.post(id, 'reanudar'); }
  finalizar(id: number)     { return this.post(id, 'finalizar'); }
  // Confirmar con Backend el cuerpo exacto de /wo y /amonestaciones
  wo(id: number, equipoId: number) { return this.post(id, 'wo', { equipoId }); }
  amonestar(id: number, equipoId: number, motivo: string) {
    return this.post(id, 'amonestaciones', { equipoId, motivo });
  }

  cerrarAsalto(id: number, body: CerrarAsaltoRequest): Observable<unknown> {
    if (this.mock) return this.mock.cerrarAsalto(id, body);
    return this.http.put(`${this.base}/${id}/asaltos/actual`, body);
  }

  private post(id: number, ruta: string, body: object = {}): Observable<unknown> {
    if (this.mock) return this.mock.accion(id, ruta, body as any);
    return this.http.post(`${this.base}/${id}/${ruta}`, body);
  }
}

/** Simulación local hasta el día 6. Imita los 409 y mensajes del backend. */
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

  listar() { return this.ok(structuredClone(this.bd)); }

  accion(id: number, ruta: string, body: { equipoId?: number }): Observable<unknown> {
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
      case 'asaltos':
        if (c.estado !== 'en_curso' || c.asaltoAbierto)
          return this.fallo(409, 'Solo se abre un asalto en un combate en curso y sin asalto abierto.');
        c.asaltoAbierto = { numero: n + 1, esMuerteSubita: c.fase !== 'grupos' && n >= 3 };
        break;
      case 'pausa':
        if (c.estado !== 'en_curso' || c.asaltoAbierto)
          return this.fallo(409, 'La pausa técnica solo se pide entre asaltos.');
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
        if (body.equipoId !== c.equipoA?.id && body.equipoId !== c.equipoB?.id)
          return this.fallo(400, 'El equipo no pertenece al combate.');
        c.marcador = body.equipoId === c.equipoA?.id ? '0 – 2' : '2 – 0';
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
      else return this.fallo(400, 'El ganador no pertenece al combate.');
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
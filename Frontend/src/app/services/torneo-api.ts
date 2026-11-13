import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_URL = 'http://localhost:5256/api/public';

// Respuesta general del estado del torneo
export interface EstadoEvento {
  horaServidor: string;
  fase: string;
  pantallaActual: string;
  equipos: Equipo[];
  sorteo: Sorteo;
  combateEnCurso: CombateEnVivo | null;
  siguienteCombate: Combate | null;
  ultimoFinalizado: Combate | null;
  grupos: Grupo[];
  bracket: Llave[];
  podio: Podio | null;
  totalCombates: number;
}

export interface Equipo {
  id: number;
  nombre: string;
  lema: string | null;
  colorHex: string;
  logoUrl: string | null;
  grupo: string | null;
  posicionSorteo: number | null;
  robot: Robot | null;
  integrantes: Integrante[];
}

export interface Robot {
  id: number;
  equipoId: number;
  nombre: string;
  fotoUrl: string | null;
  estado: string;
}

export interface Integrante {
  id: number;
  equipoId: number;
  nombreCompleto: string;
  carne: string;
}

export interface EquipoResumen {
  id: number;
  nombre: string;
  colorHex: string;
  logoUrl: string | null;
  robot: string | null;
}

export interface Sorteo {
  realizado: boolean;
  equipos: SorteoEquipo[];
}

export interface SorteoEquipo {
  grupo: string;
  posicionSorteo: number;
  equipo: EquipoResumen;
}

export interface Asalto {
  id: number;
  numero: number;
  esMuerteSubita: boolean;
  resultado: string | null;
  ganadorId: number | null;
  fechaInicio: string;
  fechaFin: string | null;
  duracionMaximaSegundos: number;
}

export interface CombateEnVivo {
  combateId: number;
  numeroOrden: number;
  fase: string;
  faseNombre: string;
  estado: string;
  equipoA: EquipoResumen;
  equipoB: EquipoResumen;
  asaltosA: number;
  asaltosB: number;
  totalAsaltos: number;
  tiempoRestanteSegundos: number | null;
  marcador: string;
  asaltoActual: Asalto | null;
  pausaEquipoId: number | null;
  pausaInicio: string | null;
}

export interface Combate {
  id: number;
  numeroOrden: number;
  fase: string;
  faseNombre: string;
  llave: string | null;
  grupo: string | null;
  estado: string;
  equipoA: EquipoResumen | null;
  equipoB: EquipoResumen | null;
  ganadorId: number | null;
  ganador: EquipoResumen | null;
  tipoResultado: string | null;
  marcador: string;
  fechaInicio: string | null;
  fechaFin: string | null;
  asaltos: Asalto[];
}

export interface Grupo {
  nombre: string;
  posiciones: Posicion[];
}

export interface Posicion {
  posicion: number;
  equipo: EquipoResumen;
  jugados: number;
  ganados: number;
  empatados: number;
  perdidos: number;
  asaltosGanados: number;
  asaltosPerdidos: number;
  diferenciaAsaltos: number;
  puntos: number;
}

export interface Llave {
  combateId: number;
  numeroOrden: number;
  fase: string;
  llave: string | null;
  equipoAId: number | null;
  equipoANombre: string;
  equipoA: EquipoResumen | null;
  equipoBId: number | null;
  equipoBNombre: string;
  equipoB: EquipoResumen | null;
  estado: string;
  marcador: string | null;
  ganadorId: number | null;
}

export interface Podio {
  primero: EquipoResumen | null;
  segundo: EquipoResumen | null;
  tercero: EquipoResumen | null;
}

@Injectable({
  providedIn: 'root'
})
export class TorneoApi {
  private http = inject(HttpClient);

  obtenerEstadoEvento(): Observable<EstadoEvento> {
    return this.http.get<EstadoEvento>(
      `${API_URL}/estado-evento`
    );
  }
}

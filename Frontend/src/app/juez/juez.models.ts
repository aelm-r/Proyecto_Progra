export type EstadoCombate = 'programado' | 'en_curso' | 'en_pausa' | 'finalizado' | 'cancelado';

export type ResultadoAsalto =
  | 'reventon' | 'autogolpe' | 'fuera_arena' | 'inmovil'
  | 'sancion' | 'nulo' | 'decision_juez';

export interface EquipoResumen {
  id: number;
  nombre: string;
  colorHex: string;
}

// Ajustar al Swagger del backend (día 3)
export interface CombateResponse {
  id: number;
  numeroOrden: number;
  fase: string; // grupos | semifinal | tercer_lugar | final
  estado: EstadoCombate;
  equipoA: EquipoResumen | null;
  equipoB: EquipoResumen | null;
  marcador: string; // lo calcula el servidor, ej. "2 – 1"
  asaltoAbierto: { numero: number; esMuerteSubita: boolean } | null;
}

// Los DTO de entrada nunca llevan id del juez, fechas ni segundos
export interface CerrarAsaltoRequest {
  resultado: ResultadoAsalto;
  ganadorId?: number;
}

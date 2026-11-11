// Modelos iguales a los ResponseDTO / RequestDTO del Swagger (camelCase).
export type EstadoRobot = 'activo' | 'en_reparacion' | 'descalificado';
export type EstadoCombate = 'programado' | 'en_curso' | 'en_pausa' | 'finalizado' | 'cancelado';

export interface IntegranteResponse { id: number; nombreCompleto: string; carne: string; }
export interface RobotResponse { id: number; nombre: string; fotoUrl?: string; estado: EstadoRobot; }
export interface EquipoResponse {
  id: number; nombre: string; lema?:string; colorHex: string;
  integrantes: IntegranteResponse[]; robot: RobotResponse | null;
}
export interface EquipoRequest { nombre: string; lema?:string; colorHex: string; }
export interface IntegranteRequest { nombreCompleto: string; carne: string; }

export interface GrupoResponse {
  nombre: string;
  equipos: { posicion: string; equipoId: number; nombre: string; colorHex: string }[];
}
export interface CombateResponse {
  id: number; numeroOrden: number; fase: string;
  equipoA: string | null; equipoB: string | null;
  estado: EstadoCombate; marcador: string | null; ganador: string | null;
}
export interface CorreccionRequest { marcador: string; motivo: string; } // sin id, fechas ni segundos

export interface LoginResponse { token: string; id: number; username: string; rol: string; }

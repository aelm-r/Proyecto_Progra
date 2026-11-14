export type EstadoRobot = 'activo' | 'en_reparacion' | 'descalificado';
export type EstadoCombate = 'programado' | 'en_curso' | 'en_pausa' | 'finalizado' | 'cancelado';

export interface IntegranteResponse { id: number; equipoId: number; nombreCompleto: string; carne: string; }
export interface RobotResponse { id: number; equipoId: number; nombre: string; fotoUrl: string | null; estado: EstadoRobot; }
export interface EquipoResponse {
  id: number; nombre: string; lema: string | null; colorHex: string; logoUrl: string | null;
  grupo: string | null; posicionSorteo: number | null;
  integrantes: IntegranteResponse[]; robot: RobotResponse | null;
}
export interface EquipoRequest { nombre: string; lema: string | null; colorHex: string; logoUrl: string | null; }
export interface IntegranteRequest { nombreCompleto: string; carne: string; }
export interface RobotRequest { nombre: string; fotoUrl: string | null; }

export interface EquipoResumen { id: number; nombre: string; colorHex: string; logoUrl: string | null; robot: string | null; }

export interface SorteoResponse {
  realizado: boolean;
  equipos: { grupo: string; posicionSorteo: number; equipo: EquipoResumen }[];
}

export interface CombateResponse {
  id: number; numeroOrden: number; fase: string; faseNombre: string; llave: string | null; grupo: string | null;
  equipoA: EquipoResumen | null; equipoB: EquipoResumen | null;
  estado: EstadoCombate; marcador: string; ganadorId: number | null; ganador: EquipoResumen | null;
  tipoResultado: string | null;
}

export interface CorreccionRequest { ganadorId: number | null; motivo: string; }

export interface LoginResponse { token: string; expiracion: string; username: string; nombreCompleto: string; rol: string; }

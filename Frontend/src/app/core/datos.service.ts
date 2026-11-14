import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from './config';
import {
  CombateResponse, CorreccionRequest, EquipoRequest, EquipoResponse, EstadoRobot,
  IntegranteRequest, IntegranteResponse, RobotRequest, RobotResponse, SorteoResponse
} from './models';

@Injectable({ providedIn: 'root' })
export class DatosService {
  private http = inject(HttpClient);

  listarEquipos(): Observable<EquipoResponse[]> {
    return this.http.get<EquipoResponse[]>(`${API_URL}/equipos`);
  }

  crearEquipo(dto: EquipoRequest): Observable<EquipoResponse> {
    return this.http.post<EquipoResponse>(`${API_URL}/equipos`, dto);
  }

  actualizarEquipo(id: number, dto: EquipoRequest): Observable<EquipoResponse> {
    return this.http.put<EquipoResponse>(`${API_URL}/equipos/${id}`, dto);
  }

  eliminarEquipo(id: number): Observable<unknown> {
    return this.http.delete(`${API_URL}/equipos/${id}`);
  }

  agregarIntegrante(equipoId: number, dto: IntegranteRequest): Observable<IntegranteResponse> {
    return this.http.post<IntegranteResponse>(`${API_URL}/equipos/${equipoId}/integrantes`, dto);
  }

  crearRobot(equipoId: number, dto: RobotRequest): Observable<RobotResponse> {
    return this.http.post<RobotResponse>(`${API_URL}/equipos/${equipoId}/robot`, dto);
  }

  actualizarRobot(robotId: number, dto: RobotRequest): Observable<RobotResponse> {
    return this.http.put<RobotResponse>(`${API_URL}/robots/${robotId}`, dto);
  }

  cambiarEstadoRobot(robotId: number, estado: EstadoRobot): Observable<RobotResponse> {
    return this.http.patch<RobotResponse>(`${API_URL}/robots/${robotId}/estado`, { estado });
  }

  obtenerSorteo(): Observable<SorteoResponse> {
    return this.http.get<SorteoResponse>(`${API_URL}/public/sorteo`);
  }

  realizarSorteo(): Observable<SorteoResponse> {
    return this.http.post<SorteoResponse>(`${API_URL}/torneo/sorteo`, {});
  }

  generarSemifinales(): Observable<unknown> {
    return this.http.post(`${API_URL}/torneo/semifinales`, {});
  }

  listarCombates(): Observable<CombateResponse[]> {
    return this.http.get<CombateResponse[]>(`${API_URL}/combates`);
  }

  corregirResultado(id: number, dto: CorreccionRequest): Observable<CombateResponse> {
    return this.http.put<CombateResponse>(`${API_URL}/combates/${id}/correccion`, dto);
  }
}

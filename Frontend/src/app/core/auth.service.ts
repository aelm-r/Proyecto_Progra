import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { API_URL } from './config';
import { LoginResponse } from './models';

const CLAVE_SESION = 'admin_sesion';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${API_URL}/auth/login`, { username, password }).pipe(
      tap(r => {
        if (r.rol === 'Admin') {
          localStorage.setItem(CLAVE_SESION, JSON.stringify(r));
        }
      })
    );
  }

  salir() { localStorage.removeItem(CLAVE_SESION); }

  private sesion(): LoginResponse | null {
    try {
      const texto = localStorage.getItem(CLAVE_SESION);
      if (!texto) return null;
      const s = JSON.parse(texto) as LoginResponse;
      return Date.parse(s.expiracion) < Date.now() ? null : s;
    } catch { return null; }
  }

  token(): string | null { return this.sesion()?.token ?? null; }
  rol(): string | null { return this.sesion()?.rol ?? null; }
  nombre(): string { return this.sesion()?.nombreCompleto || this.sesion()?.username || ''; }
}

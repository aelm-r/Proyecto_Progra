import { Injectable } from '@angular/core';
import { Observable, delay, of, throwError } from 'rxjs';
import { LoginResponse } from './models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  // SIMULADO (hasta el día 6). Luego: this.http.post<LoginResponse>('/api/auth/login', { username, password })
  login(username: string, password: string): Observable<LoginResponse> {
    if (username === 'admin' && password === 'admin123') {
      const payload = { sub: '1', unique_name: username, role: 'Admin', exp: Math.floor(Date.now() / 1000) + 8 * 3600 };
      const token = [btoa('{"alg":"none"}'), btoa(JSON.stringify(payload)), 'simulado'].join('.');
      return of({ token, id: 1, username, rol: 'Admin' }).pipe(delay(300));
    }
    return throwError(() => ({ status: 401, error: { mensaje: 'Usuario o contraseña incorrectos.' } }));
  }

  guardarToken(token: string) { localStorage.setItem('token', token); }
  salir() { localStorage.removeItem('token'); }

  private claims(): any | null {
    const t = localStorage.getItem('token');
    if (!t) return null;
    try {
      const p = JSON.parse(atob(t.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return p.exp && p.exp * 1000 < Date.now() ? null : p;
    } catch { return null; }
  }

  rol(): string | null {
    const c = this.claims();
    return c ? (c.role ?? c['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null) : null;
  }
}

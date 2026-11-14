import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { mensajeDeError } from '../core/errores';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  styleUrl: '../admin/admin.css',
  template: `
    <main class="admin-app login">
      <section class="card">
        <h1>Torneo Gallito Robótico</h1>
        <p class="sub">Panel de administración</p>
        @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
        <label>Usuario <input [(ngModel)]="username" autocomplete="username"></label>
        <label>Contraseña
          <input type="password" [(ngModel)]="password" autocomplete="current-password" (keyup.enter)="entrar()">
        </label>
        <button class="btn" (click)="entrar()" [disabled]="cargando() || !username || !password">
          {{ cargando() ? 'Entrando…' : 'Entrar' }}
        </button>
      </section>
    </main>`
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  username = ''; password = '';
  error = signal(''); cargando = signal(false);

  entrar() {
    this.error.set(''); this.cargando.set(true);
    this.auth.login(this.username, this.password).subscribe({
      next: r => {
        this.cargando.set(false);
        if (r.rol !== 'Admin') { this.error.set('Este panel es solo para el operador de mesa (Admin).'); return; }
        this.router.navigate(['/admin']);
      },
      error: e => { this.cargando.set(false); this.error.set(mensajeDeError(e)); }
    });
  }
}

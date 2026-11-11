import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { REQUIERE_LOGIN } from '../core/config';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <header class="top">
      <strong>Torneo Gallito · Administración</strong>
      <nav>
        <a routerLink="equipos" routerLinkActive="activo">Equipos y robots</a>
        <a routerLink="sorteo" routerLinkActive="activo">Sorteo y semifinales</a>
        <a routerLink="correcciones" routerLinkActive="activo">Correcciones</a>
      </nav>
      @if (requiereLogin) { <button class="btn sec" (click)="salir()">Salir</button> }
    </header>
    <main class="contenido"><router-outlet /></main>`
})
export class AdminComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  requiereLogin = REQUIERE_LOGIN;
  salir() { this.auth.salir(); this.router.navigate(['/login']); }
}
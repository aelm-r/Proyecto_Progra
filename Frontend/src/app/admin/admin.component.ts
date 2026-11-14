import { Component, ViewEncapsulation, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  encapsulation: ViewEncapsulation.None,
  styleUrl: './admin.css',
  template: `
    <div class="admin-app">
      <header class="top">
        <strong>Torneo Gallito · Administración</strong>
        <nav>
          <a routerLink="equipos" routerLinkActive="activo">Equipos y robots</a>
          <a routerLink="sorteo" routerLinkActive="activo">Sorteo y semifinales</a>
          <a routerLink="correcciones" routerLinkActive="activo">Correcciones</a>
        </nav>
        <span class="usuario">{{ nombre }}</span>
        <button class="btn sec" (click)="salir()">Salir</button>
      </header>
      <main class="contenido"><router-outlet /></main>
    </div>`
})
export class AdminComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  nombre = this.auth.nombre();
  salir() { this.auth.salir(); this.router.navigate(['/login']); }
}

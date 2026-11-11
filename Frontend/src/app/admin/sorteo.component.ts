import { Component, OnInit, inject, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { DatosService } from '../core/datos.service';
import { mensajeDeError } from '../core/errores';
import { GrupoResponse } from '../core/models';

@Component({
  selector: 'app-sorteo',
  standalone: true,
  template: `
    <h2>Sorteo y semifinales</h2>
    @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    @if (ok()) { <p class="ok">{{ ok() }}</p> }

    <section class="card">
      <h3>Sorteo de grupos</h3>
      <p>Se hace una sola vez, con exactamente 8 equipos con robot, y crea los 16 combates.</p>
      <button class="btn" (click)="realizarSorteo()" [disabled]="realizado()">Realizar sorteo</button>
    </section>

    @if (grupos().length) {
      <div class="grupos">
        @for (g of grupos(); track g.nombre) {
          <section class="card">
            <h3>Grupo {{ g.nombre }}</h3>
            <ul class="posiciones">
              @for (x of g.equipos; track x.equipoId) {
                <li [style.border-left-color]="x.colorHex"><b>{{ x.posicion }}</b> {{ x.nombre }}</li>
              }
            </ul>
          </section>
        }
      </div>
    }

    <section class="card">
      <h3>Semifinales</h3>
      <p>Disponible cuando los 12 combates de grupos estén finalizados.</p>
      <button class="btn" (click)="generarSemifinales()" [disabled]="!realizado()">Generar semifinales</button>
    </section>

    <section class="card prueba">
      <h3>Solo pruebas</h3>
      <p>Finaliza los 12 combates de grupos con resultados de ejemplo (torneo simulado).</p>
      <button class="btn sec" (click)="simular()" [disabled]="!realizado()">Simular fase de grupos</button>
    </section>`
})
export class SorteoComponent implements OnInit {
  private datos = inject(DatosService);
  realizado = signal(false);
  grupos = signal<GrupoResponse[]>([]);
  error = signal(''); ok = signal('');

  ngOnInit() { this.cargar(); }

  cargar() {
    this.datos.estadoSorteo().subscribe(r => this.realizado.set(r.realizado));
    this.datos.listarGrupos().subscribe(r => this.grupos.set(r));
  }

  private ejecutar<T>(obs: Observable<T>, mensajeOk: string) {
    this.error.set(''); this.ok.set('');
    obs.subscribe({
      next: () => { this.ok.set(mensajeOk); this.cargar(); },
      error: e => this.error.set(mensajeDeError(e))
    });
  }

  realizarSorteo() {
    if (!confirm('El sorteo se hace una sola vez y no se puede deshacer. ¿Continuar?')) return;
    this.ejecutar(this.datos.realizarSorteo(), 'Sorteo realizado. Se crearon los 16 combates.');
  }
  generarSemifinales() {
    if (!confirm('¿Generar las semifinales con las posiciones actuales?')) return;
    this.ejecutar(this.datos.generarSemifinales(), 'Semifinales generadas.');
  }
  simular() { this.ejecutar(this.datos.simularFaseDeGrupos(), 'Fase de grupos simulada.'); }
}

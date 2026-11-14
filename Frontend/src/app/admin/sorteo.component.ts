import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { DatosService } from '../core/datos.service';
import { mensajeDeError } from '../core/errores';
import { CombateResponse, SorteoResponse } from '../core/models';

type Accion = 'sorteo' | 'semifinales';

@Component({
  selector: 'app-sorteo-admin',
  standalone: true,
  template: `
    <h2>Sorteo y semifinales</h2>
    @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    @if (ok()) { <p class="ok">{{ ok() }}</p> }

    <section class="card">
      <h3>Sorteo de grupos</h3>
      <p>Se hace una sola vez, con exactamente 8 equipos con robot activo, y crea los 16 combates.
         Al realizarlo, la TV muestra la animación del sorteo hasta que empiece el primer combate.</p>

      @if (sorteo().realizado) {
        <p class="aviso">El sorteo ya se realizó. No se puede repetir.</p>
      } @else if (porConfirmar() === 'sorteo') {
        <div class="confirmar">
          <p>El sorteo se hace una sola vez y no se puede deshacer. ¿Continuar?</p>
          <button class="btn" (click)="realizarSorteo()" [disabled]="trabajando()">Sí, realizar sorteo</button>
          <button class="btn sec" (click)="porConfirmar.set(null)">Cancelar</button>
        </div>
      } @else {
        <button class="btn" (click)="pedir('sorteo')">Realizar sorteo</button>
      }
    </section>

    @if (sorteo().realizado) {
      <div class="grupos">
        @for (g of grupos(); track g.nombre) {
          <section class="card">
            <h3>Grupo {{ g.nombre }}</h3>
            <ul class="posiciones">
              @for (x of g.equipos; track x.equipo.id) {
                <li [style.border-left-color]="x.equipo.colorHex"><b>{{ x.grupo }}{{ x.posicionSorteo }}</b> {{ x.equipo.nombre }}</li>
              }
            </ul>
          </section>
        }
      </div>
    }

    <section class="card">
      <h3>Semifinales</h3>
      <p>Cruzan 1.º A vs 2.º B y 1.º B vs 2.º A según la tabla de posiciones.
         Se pueden generar cuando los 12 combates de grupos estén finalizados ({{ gruposFinalizados() }} de 12).</p>

      @if (motivoSemifinales(); as motivo) {
        <p class="aviso">{{ motivo }}</p>
      } @else if (porConfirmar() === 'semifinales') {
        <div class="confirmar">
          <p>¿Generar las semifinales con las posiciones actuales?</p>
          <button class="btn" (click)="generarSemifinales()" [disabled]="trabajando()">Sí, generar semifinales</button>
          <button class="btn sec" (click)="porConfirmar.set(null)">Cancelar</button>
        </div>
      } @else {
        <button class="btn" (click)="pedir('semifinales')">
          {{ semifinalesAsignadas() ? 'Volver a generar semifinales' : 'Generar semifinales' }}
        </button>
      }

      @if (semifinalesAsignadas()) {
        <ul>
          @for (c of semifinales(); track c.id) {
            <li>{{ c.llave }}: {{ c.equipoA?.nombre }} vs {{ c.equipoB?.nombre }} · {{ c.estado }}</li>
          }
        </ul>
      }
    </section>`
})
export class SorteoComponent implements OnInit {
  private datos = inject(DatosService);
  sorteo = signal<SorteoResponse>({ realizado: false, equipos: [] });
  combates = signal<CombateResponse[]>([]);
  error = signal(''); ok = signal('');
  porConfirmar = signal<Accion | null>(null);
  trabajando = signal(false);

  grupos = computed(() => ['A', 'B'].map(nombre => ({
    nombre,
    equipos: this.sorteo().equipos
      .filter(e => e.grupo === nombre)
      .sort((a, b) => a.posicionSorteo - b.posicionSorteo)
  })));
  gruposFinalizados = computed(() =>
    this.combates().filter(c => c.fase === 'grupos' && (c.estado === 'finalizado' || c.estado === 'cancelado')).length);
  semifinales = computed(() => this.combates().filter(c => c.fase === 'semifinal'));
  semifinalesAsignadas = computed(() => this.semifinales().some(c => c.equipoA && c.equipoB));

  motivoSemifinales = computed(() => {
    if (!this.sorteo().realizado) return 'Primero hay que realizar el sorteo.';
    if (this.gruposFinalizados() < 12) return `Faltan ${12 - this.gruposFinalizados()} combates de grupos por finalizar.`;
    if (this.semifinales().some(c => c.estado !== 'programado')) return 'Las semifinales ya empezaron; no se pueden volver a generar.';
    return null;
  });

  ngOnInit() { this.cargar(); }

  cargar() {
    this.datos.obtenerSorteo().subscribe({
      next: r => this.sorteo.set(r),
      error: e => this.error.set(mensajeDeError(e))
    });
    this.datos.listarCombates().subscribe({ next: r => this.combates.set(r) });
  }

  pedir(accion: Accion) {
    this.error.set(''); this.ok.set('');
    this.porConfirmar.set(accion);
  }

  private ejecutar<T>(obs: Observable<T>, mensajeOk: string) {
    this.error.set(''); this.ok.set('');
    this.trabajando.set(true);
    obs.subscribe({
      next: () => { this.trabajando.set(false); this.porConfirmar.set(null); this.ok.set(mensajeOk); this.cargar(); },
      error: e => { this.trabajando.set(false); this.porConfirmar.set(null); this.error.set(mensajeDeError(e)); }
    });
  }

  realizarSorteo() {
    this.ejecutar(this.datos.realizarSorteo(), 'Sorteo realizado. Se crearon los 16 combates.');
  }
  generarSemifinales() {
    this.ejecutar(this.datos.generarSemifinales(), 'Semifinales generadas.');
  }
}

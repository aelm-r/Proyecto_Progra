import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatosService } from '../core/datos.service';
import { mensajeDeError } from '../core/errores';
import { CombateResponse } from '../core/models';

@Component({
  selector: 'app-correcciones',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h2>Corrección de resultados</h2>
    <p>Solo el operador de mesa corrige un resultado finalizado, con motivo escrito (10 a 300 caracteres).
       Cada corrección queda en la bitácora de auditoría.</p>
    @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    @if (ok()) { <p class="ok">{{ ok() }}</p> }

    @if (finalizados().length === 0) {
      <p class="vacio card">Todavía no hay combates finalizados.</p>
    } @else {
      <div class="tabla-scroll">
        <table>
          <thead><tr><th>#</th><th>Fase</th><th>Combate</th><th>Marcador</th><th>Ganador</th><th></th></tr></thead>
          <tbody>
            @for (c of finalizados(); track c.id) {
              <tr [class.sel]="seleccionado()?.id === c.id">
                <td>{{ c.numeroOrden }}</td><td>{{ c.faseNombre }}</td>
                <td>{{ c.equipoA?.nombre }} vs {{ c.equipoB?.nombre }}</td>
                <td>{{ c.marcador }}</td><td>{{ c.ganador?.nombre ?? 'Empate' }}</td>
                <td><button class="btn sec" (click)="elegir(c)">Corregir</button></td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }

    @if (seleccionado(); as c) {
      <section class="card">
        <h3>Corregir combate {{ c.numeroOrden }}: {{ c.equipoA?.nombre }} vs {{ c.equipoB?.nombre }}</h3>
        <label>Ganador oficial
          <select [(ngModel)]="ganador">
            <option [ngValue]="c.equipoA?.id">{{ c.equipoA?.nombre }}</option>
            <option [ngValue]="c.equipoB?.id">{{ c.equipoB?.nombre }}</option>
            @if (c.fase === 'grupos') { <option [ngValue]="null">Empate</option> }
          </select>
        </label>
        <label>Motivo
          <textarea rows="3" maxlength="300" [(ngModel)]="motivo"></textarea>
        </label>
        <small [class.mal]="!motivoValido">{{ motivo.trim().length }}/300 (mínimo 10)</small>
        <div class="fila">
          <button class="btn" (click)="guardar(c)" [disabled]="!motivoValido">Guardar corrección</button>
          <button class="btn sec" (click)="seleccionado.set(null)">Cancelar</button>
        </div>
      </section>
    }`
})
export class CorreccionesComponent implements OnInit {
  private datos = inject(DatosService);
  combates = signal<CombateResponse[]>([]);
  finalizados = computed(() => this.combates().filter(c => c.estado === 'finalizado'));
  seleccionado = signal<CombateResponse | null>(null);
  error = signal(''); ok = signal('');
  ganador: number | null | undefined = null; motivo = '';

  get motivoValido() { const n = this.motivo.trim().length; return n >= 10 && n <= 300; }

  ngOnInit() { this.cargar(); }
  cargar() {
    this.datos.listarCombates().subscribe({
      next: r => this.combates.set(r),
      error: e => this.error.set(mensajeDeError(e))
    });
  }

  elegir(c: CombateResponse) {
    this.seleccionado.set(c); this.ganador = c.ganadorId; this.motivo = '';
    this.error.set(''); this.ok.set('');
  }

  guardar(c: CombateResponse) {
    this.error.set(''); this.ok.set('');
    this.datos.corregirResultado(c.id, { ganadorId: this.ganador ?? null, motivo: this.motivo.trim() }).subscribe({
      next: () => { this.ok.set('Resultado corregido.'); this.seleccionado.set(null); this.cargar(); },
      error: e => this.error.set(mensajeDeError(e))
    });
  }
}

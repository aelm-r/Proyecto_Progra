import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { DatosService } from '../core/datos.service';
import { mensajeDeError } from '../core/errores';
import { EquipoResponse, EstadoRobot } from '../core/models';

@Component({
  selector: 'app-equipos',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h2>Equipos, integrantes y robots</h2>
    @if (sorteoHecho()) { <p class="aviso">El sorteo ya se realizó: no se pueden crear ni eliminar equipos.</p> }
    @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    @if (ok()) { <p class="ok">{{ ok() }}</p> }

    <section class="card">
      <h3>Nuevo equipo</h3>
      <div class="fila">
        <input [(ngModel)]="nuevo.nombre" placeholder="Nombre" [disabled]="sorteoHecho()">
        <input type="color" [(ngModel)]="nuevo.colorHex" [disabled]="sorteoHecho()" aria-label="Color del equipo">
        <button class="btn" (click)="crear()" [disabled]="sorteoHecho() || !nuevo.nombre.trim()">Crear equipo</button>
      </div>
    </section>

    <p class="cuenta">{{ equipos().length }} de 8 equipos · {{ conRobot() }} con robot</p>

    @for (e of equipos(); track e.id) {
      <article class="card equipo" [style.border-left-color]="e.colorHex">
        <header>
          @if (editandoId() === e.id) {
            <input [(ngModel)]="edicion.nombre" placeholder="Nombre" aria-label="Nombre del equipo">
            <input type="color" [(ngModel)]="edicion.colorHex" aria-label="Color del equipo">
            <button class="btn" (click)="guardarEdicion(e.id)" [disabled]="!edicion.nombre.trim()">Guardar</button>
            <button class="btn sec" (click)="cancelarEdicion()">Cancelar</button>
          } @else {
            <h3>{{ e.nombre }}</h3>
            <span class="lema"></span>
            <button class="btn sec" (click)="editar(e)">Editar</button>
            <button class="btn peligro" (click)="eliminar(e)" [disabled]="sorteoHecho()">Eliminar</button>
          }
        </header>
        <div class="dos">
          <div>
            <h4>Integrantes ({{ e.integrantes.length }})</h4>
            <ul>
              @for (i of e.integrantes; track i.id) { <li>{{ i.nombreCompleto }} <small>· {{ i.carne }}</small></li> }
              @empty { <li class="vacio">Sin integrantes</li> }
            </ul>
            <div class="fila">
              <input #n placeholder="Nombre completo">
              <input #c placeholder="Carné">
              <button class="btn sec" (click)="agregarIntegrante(e.id, n.value, c.value); n.value=''; c.value=''">Agregar</button>
            </div>
          </div>
          <div>
            <h4>Robot</h4>
            @if (e.robot) {
              <p><strong>{{ e.robot.nombre }}</strong></p>
              <label>Estado
                <select (change)="cambiarEstado(e.robot.id, $any($event.target).value)">
                  @for (s of estados; track s) { <option [value]="s" [selected]="s === e.robot.estado">{{ s }}</option> }
                </select>
              </label>
            } @else { <p class="vacio">Sin robot</p> }
            <div class="fila">
              <input #r placeholder="Nombre del robot">
              <button class="btn sec" (click)="guardarRobot(e.id, r.value); r.value=''">
                {{ e.robot ? 'Renombrar' : 'Registrar robot' }}
              </button>
            </div>
          </div>
        </div>
      </article>
    }`
})
export class EquiposComponent implements OnInit {
  private datos = inject(DatosService);
  equipos = signal<EquipoResponse[]>([]);
  sorteoHecho = signal(false);
  error = signal(''); ok = signal('');
  conRobot = computed(() => this.equipos().filter(e => e.robot).length);
  estados: EstadoRobot[] = ['activo', 'en_reparacion', 'descalificado'];
  nuevo = { nombre: '', colorHex: '#1e88e5' };
  editandoId = signal<number | null>(null);
  edicion = { nombre: '', colorHex: '#1e88e5' };

  ngOnInit() { this.cargar(); }

  cargar() {
    this.datos.listarEquipos().subscribe(r => this.equipos.set(r));
    this.datos.estadoSorteo().subscribe(r => this.sorteoHecho.set(r.realizado));
  }

  private ejecutar<T>(obs: Observable<T>, mensajeOk: string, alExito?: () => void) {
    this.error.set(''); this.ok.set('');
    obs.subscribe({
      next: () => { this.ok.set(mensajeOk); alExito?.(); this.cargar(); },
      error: e => this.error.set(mensajeDeError(e))
    });
  }

  crear() {
    this.ejecutar(this.datos.crearEquipo({ ...this.nuevo }), 'Equipo creado.',
      () => (this.nuevo = { nombre: '', colorHex: '#1e88e5' }));
  }
  eliminar(e: EquipoResponse) {
    if (!confirm(`¿Eliminar el equipo «${e.nombre}»?`)) return;
    this.ejecutar(this.datos.eliminarEquipo(e.id), 'Equipo eliminado.');
  }
  editar(e: EquipoResponse) {
    this.editandoId.set(e.id);
    this.edicion = { nombre: e.nombre, colorHex: e.colorHex };
  }
  cancelarEdicion() { this.editandoId.set(null); }
  guardarEdicion(id: number) {
    this.ejecutar(this.datos.actualizarEquipo(id, { ...this.edicion }), 'Equipo actualizado.',
      () => this.editandoId.set(null));
  }
  agregarIntegrante(equipoId: number, nombreCompleto: string, carne: string) {
    this.ejecutar(this.datos.agregarIntegrante(equipoId, { nombreCompleto, carne }), 'Integrante agregado.');
  }
  guardarRobot(equipoId: number, nombre: string) {
    this.ejecutar(this.datos.guardarRobot(equipoId, nombre), 'Robot guardado.');
  }
  cambiarEstado(robotId: number, estado: EstadoRobot) {
    this.ejecutar(this.datos.cambiarEstadoRobot(robotId, estado), 'Estado del robot actualizado.');
  }
}
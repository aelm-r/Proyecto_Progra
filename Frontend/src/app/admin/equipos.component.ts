import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { DatosService } from '../core/datos.service';
import { mensajeDeError } from '../core/errores';
import { EquipoRequest, EquipoResponse, EstadoRobot } from '../core/models';

const equipoVacio = (): EquipoRequest => ({ nombre: '', lema: '', colorHex: '#1e88e5', logoUrl: '' });

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
        <input [(ngModel)]="nuevo.lema" placeholder="Lema (opcional)" [disabled]="sorteoHecho()">
        <input [(ngModel)]="nuevo.logoUrl" placeholder="URL del logo (opcional)" [disabled]="sorteoHecho()">
        <input type="color" [(ngModel)]="nuevo.colorHex" [disabled]="sorteoHecho()" aria-label="Color del equipo">
        <button class="btn" (click)="crear()" [disabled]="sorteoHecho() || !nuevo.nombre.trim()">Crear equipo</button>
      </div>
    </section>

    <p class="cuenta">{{ equipos().length }} de 8 equipos · {{ listos() }} con robot activo</p>

    @for (e of equipos(); track e.id) {
      <article class="card equipo" [style.border-left-color]="e.colorHex">
        <header>
          @if (editandoId() === e.id) {
            <input [(ngModel)]="edicion.nombre" placeholder="Nombre" aria-label="Nombre del equipo">
            <input [(ngModel)]="edicion.lema" placeholder="Lema" aria-label="Lema del equipo">
            <input [(ngModel)]="edicion.logoUrl" placeholder="URL del logo" aria-label="Logo del equipo">
            <input type="color" [(ngModel)]="edicion.colorHex" aria-label="Color del equipo">
            <button class="btn" (click)="guardarEdicion(e.id)" [disabled]="!edicion.nombre.trim()">Guardar</button>
            <button class="btn sec" (click)="cancelarEdicion()">Cancelar</button>
          } @else {
            <h3>{{ e.nombre }}</h3>
            @if (e.grupo) { <span>· Grupo {{ e.grupo }}{{ e.posicionSorteo }}</span> }
            <span class="lema">{{ e.lema }}</span>
            <button class="btn sec" (click)="editar(e)">Editar</button>
            @if (!sorteoHecho()) {
              <button class="btn peligro" (click)="porEliminar.set(e.id)">Eliminar</button>
            }
          }
        </header>
        @if (porEliminar() === e.id) {
          <div class="confirmar">
            <p>¿Eliminar el equipo «{{ e.nombre }}» con sus integrantes y su robot?</p>
            <button class="btn peligro" (click)="eliminar(e)">Sí, eliminar</button>
            <button class="btn sec" (click)="porEliminar.set(null)">Cancelar</button>
          </div>
        }
        <div class="dos">
          <div>
            <h4>Integrantes ({{ e.integrantes.length }})</h4>
            <ul>
              @for (i of e.integrantes; track i.id) { <li>{{ i.nombreCompleto }} <small>· {{ i.carne }}</small></li> }
              @empty { <li class="vacio">Sin integrantes</li> }
            </ul>
            <div class="fila">
              <input #n placeholder="Nombre completo">
              <input #c placeholder="Carné (solo números)">
              <button class="btn sec" (click)="agregarIntegrante(e.id, n.value, c.value); n.value=''; c.value=''">Agregar</button>
            </div>
          </div>
          <div>
            <h4>Robot</h4>
            @if (e.robot; as robot) {
              <p><strong>{{ robot.nombre }}</strong></p>
              <label>Estado
                <select #estado [disabled]="robot.estado === 'descalificado'" (change)="cambiarEstado(robot.id, $any(estado.value))">
                  @for (s of estados; track s.valor) {
                    <option [value]="s.valor" [selected]="s.valor === robot.estado">{{ s.texto }}</option>
                  }
                </select>
              </label>
              @if (robot.estado === 'descalificado') {
                <p class="vacio">Un robot descalificado ya no puede volver a competir.</p>
              }
              @if (porDescalificar() === robot.id) {
                <div class="confirmar">
                  <p>Un robot descalificado ya no puede volver a competir. ¿Descalificar a «{{ robot.nombre }}»?</p>
                  <button class="btn peligro" (click)="descalificar(robot.id)">Sí, descalificar</button>
                  <button class="btn sec" (click)="porDescalificar.set(null); estado.value = robot.estado">Cancelar</button>
                </div>
              }
            } @else { <p class="vacio">Sin robot</p> }
            <div class="fila">
              <input #r placeholder="Nombre del robot">
              <button class="btn sec" (click)="guardarRobot(e, r.value); r.value=''">
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
  listos = computed(() => this.equipos().filter(e => e.robot?.estado === 'activo').length);
  estados: { valor: EstadoRobot; texto: string }[] = [
    { valor: 'activo', texto: 'Activo' },
    { valor: 'en_reparacion', texto: 'En reparación' },
    { valor: 'descalificado', texto: 'Descalificado' }
  ];
  nuevo = equipoVacio();
  editandoId = signal<number | null>(null);
  edicion = equipoVacio();

  ngOnInit() { this.cargar(); }

  cargar() {
    this.datos.listarEquipos().subscribe({
      next: r => this.equipos.set(r),
      error: e => this.error.set(mensajeDeError(e))
    });
    this.datos.obtenerSorteo().subscribe(r => this.sorteoHecho.set(r.realizado));
  }

  private ejecutar<T>(obs: Observable<T>, mensajeOk: string, alExito?: () => void) {
    this.error.set(''); this.ok.set('');
    obs.subscribe({
      next: () => { this.ok.set(mensajeOk); alExito?.(); this.cargar(); },
      error: e => this.error.set(mensajeDeError(e))
    });
  }

  private limpiar(dto: EquipoRequest): EquipoRequest {
    return {
      nombre: dto.nombre.trim(),
      lema: dto.lema?.trim() || null,
      colorHex: dto.colorHex.toUpperCase(),
      logoUrl: dto.logoUrl?.trim() || null
    };
  }

  crear() {
    this.ejecutar(this.datos.crearEquipo(this.limpiar(this.nuevo)), 'Equipo creado.',
      () => (this.nuevo = equipoVacio()));
  }
  porEliminar = signal<number | null>(null);
  porDescalificar = signal<number | null>(null);

  eliminar(e: EquipoResponse) {
    this.porEliminar.set(null);
    this.ejecutar(this.datos.eliminarEquipo(e.id), 'Equipo eliminado.');
  }
  editar(e: EquipoResponse) {
    this.editandoId.set(e.id);
    this.edicion = { nombre: e.nombre, lema: e.lema ?? '', colorHex: e.colorHex, logoUrl: e.logoUrl ?? '' };
  }
  cancelarEdicion() { this.editandoId.set(null); }
  guardarEdicion(id: number) {
    this.ejecutar(this.datos.actualizarEquipo(id, this.limpiar(this.edicion)), 'Equipo actualizado.',
      () => this.editandoId.set(null));
  }
  agregarIntegrante(equipoId: number, nombreCompleto: string, carne: string) {
    this.ejecutar(this.datos.agregarIntegrante(equipoId, { nombreCompleto: nombreCompleto.trim(), carne: carne.trim() }),
      'Integrante agregado.');
  }
  guardarRobot(e: EquipoResponse, nombre: string) {
    const dto = { nombre: nombre.trim(), fotoUrl: e.robot?.fotoUrl ?? null };
    const accion = e.robot ? this.datos.actualizarRobot(e.robot.id, dto) : this.datos.crearRobot(e.id, dto);
    this.ejecutar(accion, 'Robot guardado.');
  }
  cambiarEstado(robotId: number, estado: EstadoRobot) {
    if (estado === 'descalificado') {
      this.porDescalificar.set(robotId);
      return;
    }
    this.ejecutar(this.datos.cambiarEstadoRobot(robotId, estado), 'Estado del robot actualizado.');
  }
  descalificar(robotId: number) {
    this.porDescalificar.set(null);
    this.ejecutar(this.datos.cambiarEstadoRobot(robotId, 'descalificado'), 'Robot descalificado.');
  }
}

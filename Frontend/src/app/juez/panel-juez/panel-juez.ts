import { HttpErrorResponse } from '@angular/common/http';
import { Component, HostListener, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable, finalize } from 'rxjs';
import { CombateResponse, EquipoResumen, EstadoCombate, ResultadoAsalto } from '../juez.models';
import { JuezService } from '../juez.service';

type TipoDialogo = 'finalizar' | 'wo' | 'asalto' | 'amonestacion';

@Component({
  selector: 'app-panel-juez',
  imports: [FormsModule],
  templateUrl: './panel-juez.html',
  styleUrl: './panel-juez.scss',
})
export class PanelJuez implements OnInit {
  private api = inject(JuezService);

  combates = signal<CombateResponse[]>([]);
  combate = signal<CombateResponse | null>(null);
  error = signal<string | null>(null);
  cargando = signal(false);
  dialogo = signal<TipoDialogo | null>(null);

  // Formulario de los diálogos
  equipoSel = signal<number | null>(null);
  resultadoSel = signal<ResultadoAsalto | null>(null);
  motivo = signal('');

  readonly resultados: { valor: ResultadoAsalto; texto: string }[] = [
    { valor: 'reventon', texto: 'Reventón' },
    { valor: 'autogolpe', texto: 'Autogolpe' },
    { valor: 'fuera_arena', texto: 'Fuera de la arena' },
    { valor: 'inmovil', texto: 'Inmóvil' },
    { valor: 'sancion', texto: 'Sanción' },
    { valor: 'decision_juez', texto: 'Decisión del juez' },
    { valor: 'nulo', texto: 'Nulo (sin ganador)' },
  ];

  equipos = computed(() => {
    const c = this.combate();
    return [c?.equipoA, c?.equipoB].filter((e): e is EquipoResumen => !!e);
  });

  esMuerteSubita = computed(() => !!this.combate()?.asaltoAbierto?.esMuerteSubita);

  /** Solo refleja el estado que manda la API; el backend valida de verdad. */
  puede = computed(() => {
    const c = this.combate();
    const libre = !!c && !this.cargando();
    const definido = !!c?.equipoA && !!c?.equipoB;
    const estado = c?.estado;
    const enCurso = estado === 'en_curso';
    const abierto = !!c?.asaltoAbierto;
    return {
      iniciar: libre && definido && estado === 'programado',
      wo: libre && definido && estado === 'programado',
      iniciarAsalto: libre && enCurso && !abierto,
      cerrarAsalto: libre && enCurso && abierto,
      amonestar: libre && enCurso,
      pausar: libre && enCurso && !abierto, // la pausa es solo entre asaltos
      reanudar: libre && estado === 'en_pausa',
      finalizar: libre && enCurso && !abierto,
    };
  });

  dialogoValido = computed(() => {
    const eq = this.equipoSel();
    const res = this.resultadoSel();
    switch (this.dialogo()) {
      case 'finalizar': return true;
      case 'wo': return eq !== null;
      case 'amonestacion': return eq !== null && this.motivo().trim().length >= 3;
      case 'asalto': return res === 'nulo' || (res !== null && eq !== null);
      default: return false;
    }
  });

  ngOnInit(): void { this.recargar(); }

  @HostListener('document:keydown.escape')
  cerrarDialogo(): void { this.dialogo.set(null); }

  etiquetaEstado(e: EstadoCombate): string {
    return { programado: 'Programado', en_curso: 'En curso', en_pausa: 'En pausa',
             finalizado: 'Finalizado', cancelado: 'Cancelado' }[e];
  }

  seleccionar(id: number): void {
    this.error.set(null);
    this.combate.set(this.combates().find((c) => c.id === id) ?? null);
  }

  // --- Acciones directas ---
  iniciar(): void       { this.ejecutar((id) => this.api.iniciar(id)); }
  iniciarAsalto(): void { this.ejecutar((id) => this.api.iniciarAsalto(id)); }
  pausar(): void        { this.ejecutar((id) => this.api.pausar(id)); }
  reanudar(): void      { this.ejecutar((id) => this.api.reanudar(id)); }

  // --- Diálogos ---
  abrir(tipo: TipoDialogo, resultado: ResultadoAsalto | null = null): void {
    this.error.set(null);
    this.equipoSel.set(null);
    this.resultadoSel.set(resultado);
    this.motivo.set('');
    this.dialogo.set(tipo);
  }

  confirmar(): void {
    const tipo = this.dialogo();
    if (!tipo || !this.dialogoValido()) return;
    const cerrar = () => this.dialogo.set(null);
    const equipoId = this.equipoSel();
    const resultado = this.resultadoSel();

    switch (tipo) {
      case 'finalizar':
        this.ejecutar((id) => this.api.finalizar(id), cerrar);
        break;
      case 'wo':
        this.ejecutar((id) => this.api.wo(id, equipoId!), cerrar);
        break;
      case 'amonestacion':
        this.ejecutar((id) => this.api.amonestar(id, equipoId!, this.motivo().trim()), cerrar);
        break;
      case 'asalto':
        this.ejecutar((id) => this.api.cerrarAsalto(id, {
          resultado: resultado!,
          ...(resultado === 'nulo' ? {} : { ganadorId: equipoId! }),
        }), cerrar);
        break;
    }
  }

  // --- Internos ---
  private ejecutar(accion: (id: number) => Observable<unknown>, alExito?: () => void): void {
    const c = this.combate();
    if (!c) return;
    this.cargando.set(true);
    this.error.set(null);
    accion(c.id).pipe(finalize(() => this.cargando.set(false))).subscribe({
      next: () => { alExito?.(); this.recargar(); },
      error: (e: HttpErrorResponse) => this.error.set(this.mensaje(e)), // tal cual lo manda el backend
    });
  }

  /** La fuente de verdad es el GET: tras cada acción se vuelve a pedir. */
  private recargar(): void {
    this.api.listar().subscribe({
      next: (lista) => {
        this.combates.set(lista);
        const id = this.combate()?.id;
        this.combate.set(lista.find((c) => c.id === id) ?? this.elegirInicial(lista));
      },
      error: (e: HttpErrorResponse) => this.error.set(this.mensaje(e)),
    });
  }

  private elegirInicial(lista: CombateResponse[]): CombateResponse | null {
    return lista.find((c) => c.estado === 'en_curso' || c.estado === 'en_pausa')
      ?? lista.find((c) => c.estado === 'programado' && c.equipoA && c.equipoB)
      ?? lista[0] ?? null;
  }

  private mensaje(e: HttpErrorResponse): string {
    if (typeof e.error === 'string' && e.error) return e.error;
    return e.error?.mensaje ?? e.error?.message ?? 'No se pudo completar la acción.';
  }
}
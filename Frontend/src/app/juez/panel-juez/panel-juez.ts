import { HttpErrorResponse } from '@angular/common/http';
import { Component, HostListener, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable, finalize } from 'rxjs';
import {
  CombateResponse, EquipoResumen, EstadoCombate, JuezApi, ResultadoAsalto,
} from '../../services/juez-api';

type TipoDialogo = 'finalizar' | 'wo' | 'asalto' | 'amonestacion' | 'pausa';

@Component({
  selector: 'app-panel-juez',
  imports: [FormsModule],
  templateUrl: './panel-juez.html',
  styleUrl: './panel-juez.scss',
})
export class PanelJuez implements OnInit, OnDestroy {
  protected api = inject(JuezApi);
  sesion = this.api.sesion;

  combates = signal<CombateResponse[]>([]);
  combate = signal<CombateResponse | null>(null);
  error = signal<string | null>(null);
  cargando = signal(false);
  dialogo = signal<TipoDialogo | null>(null);

  // Login
  usuario = signal('');
  clave = signal('');
  errorLogin = signal<string | null>(null);
  entrando = signal(false);

  // Formulario de los diálogos
  equipoSel = signal<number | null>(null);
  resultadoSel = signal<ResultadoAsalto | null>(null);
  motivo = signal('');

  // Reloj de pantalla (solo para mostrar el tiempo del asalto; el servidor valida)
  ahora = signal(Date.now());
  private reloj?: ReturnType<typeof setInterval>;

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

  /** Cuenta regresiva del asalto abierto, con fechaInicio y duración que manda la API. */
  tiempo = computed(() => {
    const a = this.combate()?.asaltoAbierto;
    if (!a?.fechaInicio || !a.duracionMaximaSegundos) return null;
    const inicio = Date.parse(a.fechaInicio);
    if (Number.isNaN(inicio)) return null;
    const transcurrido = Math.max(0, Math.floor((this.ahora() - inicio) / 1000));
    const restante = a.duracionMaximaSegundos - transcurrido;
    const texto = restante > 0
      ? `${Math.floor(restante / 60)}:${String(restante % 60).padStart(2, '0')}`
      : 'Tiempo cumplido';
    return { cumplido: restante <= 0, texto };
  });

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
      case 'pausa': return eq !== null;
      case 'amonestacion': return eq !== null && this.motivo().trim().length >= 1;
      case 'asalto': return res === 'nulo' || (res !== null && eq !== null);
      default: return false;
    }
  });

  ngOnInit(): void {
    this.reloj = setInterval(() => this.ahora.set(Date.now()), 1000);
    if (this.sesion()) this.recargar();
  }

  ngOnDestroy(): void {
    if (this.reloj) clearInterval(this.reloj);
  }

  @HostListener('document:keydown.escape')
  cerrarDialogo(): void { this.dialogo.set(null); }

  // --- Sesión ---
  entrar(): void {
    if (!this.usuario().trim() || !this.clave()) return;
    this.entrando.set(true);
    this.errorLogin.set(null);
    this.api.login(this.usuario().trim(), this.clave()).pipe(
      finalize(() => this.entrando.set(false)),
    ).subscribe({
      next: () => { this.clave.set(''); this.error.set(null); this.recargar(); },
      error: (e: HttpErrorResponse) => this.errorLogin.set(this.mensaje(e)),
    });
  }

  salir(): void {
    this.api.salir();
    this.combates.set([]);
    this.combate.set(null);
    this.dialogo.set(null);
    this.error.set(null);
  }

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
      case 'pausa':
        this.ejecutar((id) => this.api.pausar(id, equipoId!), cerrar);
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
      error: (e: HttpErrorResponse) => this.falla(e),
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
      error: (e: HttpErrorResponse) => this.falla(e),
    });
  }

  private falla(e: HttpErrorResponse): void {
    if (e.status === 401) {
      // El servicio ya cerró la sesión: se vuelve a la pantalla de login.
      this.dialogo.set(null);
      this.errorLogin.set('Tu sesión terminó. Entra de nuevo.');
      return;
    }
    this.error.set(this.mensaje(e));
  }

  private elegirInicial(lista: CombateResponse[]): CombateResponse | null {
    return lista.find((c) => c.estado === 'en_curso' || c.estado === 'en_pausa')
      ?? lista.find((c) => c.estado === 'programado' && c.equipoA && c.equipoB)
      ?? lista[0] ?? null;
  }

  /** Mensaje del backend tal cual (más el detalle de validación si viene). */
  private mensaje(e: HttpErrorResponse): string {
    if (e.status === 0) {
      return 'No se pudo conectar con el servidor. Revisa que el backend esté corriendo y la dirección en juez.config.ts.';
    }
    if (typeof e.error === 'string' && e.error) return e.error;
    const base = e.error?.mensaje ?? e.error?.message ?? 'No se pudo completar la acción.';
    const errores = e.error?.errores as Record<string, string[]> | undefined;
    const detalle = errores ? Object.values(errores).flat().join(' ') : '';
    return detalle ? `${base} ${detalle}` : base;
  }
}

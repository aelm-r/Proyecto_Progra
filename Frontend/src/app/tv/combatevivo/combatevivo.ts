import { Component, OnDestroy, computed, effect, input, signal } from '@angular/core';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-combatevivo',
  imports: [],
  templateUrl: './combatevivo.html',
  styleUrl: './combatevivo.css'
})
export class Combatevivo implements OnDestroy {

  estado = input.required<EstadoEvento>();

  titulo = 'COMBATE EN CURSO';

  private ahora = signal(Date.now());
  private desfaseServidor = 0;
  private reloj = setInterval(() => this.ahora.set(Date.now()), 250);

  constructor() {
    effect(() => {
      const hora = Date.parse(this.estado().horaServidor);
      if (!Number.isNaN(hora)) {
        this.desfaseServidor = hora - Date.now();
      }
    });
  }

  segundosRestantes = computed(() => {
    const combate = this.estado().combateEnCurso;
    const asalto = combate?.asaltoActual;
    if (!combate || !asalto) {
      return null;
    }

    const inicio = Date.parse(asalto.fechaInicio);
    if (Number.isNaN(inicio)) {
      return combate.tiempoRestanteSegundos;
    }

    const transcurridos = (this.ahora() + this.desfaseServidor - inicio) / 1000;
    return Math.max(0, Math.ceil(asalto.duracionMaximaSegundos - transcurridos));
  });

  ngOnDestroy(): void {
    clearInterval(this.reloj);
  }
}

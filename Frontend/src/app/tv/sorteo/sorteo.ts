import { Component, computed, input } from '@angular/core';
import { EstadoEvento, SorteoEquipo } from '../../services/torneo-api';

const ORDEN_GRUPOS: [string, number, number][] = [
  ['A', 1, 2], ['B', 1, 2], ['A', 3, 4], ['B', 3, 4],
  ['A', 1, 3], ['B', 1, 3], ['A', 2, 4], ['B', 2, 4],
  ['A', 1, 4], ['B', 1, 4], ['A', 2, 3], ['B', 2, 3]
];

const SEGUNDOS_POR_EQUIPO = 2.5;

@Component({
  selector: 'app-sorteo',
  imports: [],
  templateUrl: './sorteo.html',
  styleUrl: './sorteo.css'
})
export class Sorteo {

  estado = input.required<EstadoEvento>();

  titulo = 'SORTEO DE GRUPOS';

  grupoA = computed(() => this.equiposDe('A'));
  grupoB = computed(() => this.equiposDe('B'));

  totalRevelacion = 8 * SEGUNDOS_POR_EQUIPO;

  fixture = computed(() => {
    const equipos = this.estado().sorteo.equipos;
    const nombre = (grupo: string, posicion: number) =>
      equipos.find(e => e.grupo === grupo && e.posicionSorteo === posicion)?.equipo.nombre ?? '—';

    return ORDEN_GRUPOS.map(([grupo, a, b], i) => ({
      numero: i + 1,
      grupo,
      equipoA: nombre(grupo, a),
      equipoB: nombre(grupo, b)
    }));
  });

  retraso(item: SorteoEquipo): string {
    const indice = (item.posicionSorteo - 1) * 2 + (item.grupo === 'A' ? 0 : 1);
    return `${0.8 + indice * SEGUNDOS_POR_EQUIPO}s`;
  }

  private equiposDe(grupo: string): SorteoEquipo[] {
    return this.estado().sorteo.equipos
      .filter(e => e.grupo === grupo)
      .sort((a, b) => a.posicionSorteo - b.posicionSorteo);
  }
}

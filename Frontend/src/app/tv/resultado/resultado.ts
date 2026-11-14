import { Component, input } from '@angular/core';
import { EstadoEvento } from '../../services/torneo-api';

const TIPOS_RESULTADO: Record<string, string> = {
  reventon: 'GLOBO REVENTADO',
  autogolpe: 'AUTOGOLPE DEL RIVAL',
  fuera_arena: 'RIVAL FUERA DE LA ARENA',
  inmovil: 'RIVAL INMÓVIL',
  sancion: 'SANCIÓN AL RIVAL',
  decision_juez: 'DECISIÓN DEL JUEZ',
  wo: 'W.O. (NO SE PRESENTÓ EL RIVAL)',
  empate: 'EMPATE',
  nulo: 'NULO'
};

@Component({
  selector: 'app-resultado',
  imports: [],
  templateUrl: './resultado.html',
  styleUrl: './resultado.css'
})
export class Resultado {
  estado = input.required<EstadoEvento>();

  textoResultado(tipo: string | null): string {
    if (!tipo) {
      return 'NO DISPONIBLE';
    }
    return TIPOS_RESULTADO[tipo] ?? tipo.toUpperCase();
  }
}

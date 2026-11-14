import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EstadoEvento, Llave } from '../../services/torneo-api';

@Component({
  selector: 'app-eliminatoria',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './eliminatoria.html',
  styleUrl: './eliminatoria.css'
})
export class Eliminatoria {
  estado = input.required<EstadoEvento>();

  imagenFondo = input<string>('/imagenes/fondo-gallito.png');

  nombreLlave(fase: string, llave: string | null): string {
    if (llave === 'SF1') return 'Semifinal 1';
    if (llave === 'SF2') return 'Semifinal 2';
    if (fase === 'tercer_lugar') return 'Tercer lugar';
    if (fase === 'final') return 'Gran final';
    return llave ?? fase;
  }

  textoEstado(combate: Llave): string {
    if (combate.estado === 'finalizado') return combate.marcador ?? 'FINALIZADO';
    if (combate.estado === 'en_curso' || combate.estado === 'en_pausa') return `EN CURSO · ${combate.marcador ?? ''}`;
    return 'PENDIENTE';
  }
}

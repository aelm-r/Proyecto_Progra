import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-eliminatoria',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './eliminatoria.html',
  styleUrl: './eliminatoria.css'
})
export class Eliminatoria {
  estado = input.required<EstadoEvento>();

  imagenFondo = input<string>('assets/image_954b97.jpg');
}

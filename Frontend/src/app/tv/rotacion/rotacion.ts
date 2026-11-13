import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-rotacion',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rotacion.html',
  styleUrl: './rotacion.css'
})
export class Rotacion {
  estado = input.required<EstadoEvento>();

  imagenFondo = input<string>('assets/image_954b97.jpg');
}

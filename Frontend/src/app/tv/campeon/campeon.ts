import { Component, input } from '@angular/core';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-campeon',
  imports: [],
  templateUrl: './campeon.html',
  styleUrl: './campeon.css',
})
export class Campeon {
  estado = input.required<EstadoEvento>();
}

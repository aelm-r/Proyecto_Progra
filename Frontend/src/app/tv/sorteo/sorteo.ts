import { Component, input } from '@angular/core';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-sorteo',
  imports: [],
  templateUrl: './sorteo.html',
  styleUrl: './sorteo.css'
})
export class Sorteo {

  estado = input.required<EstadoEvento>();

  titulo = 'SORTEO DE EQUIPOS';

}

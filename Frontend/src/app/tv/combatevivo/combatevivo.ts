import { Component, input } from '@angular/core';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-combatevivo',
  imports: [],
  templateUrl: './combatevivo.html',
  styleUrl: './combatevivo.css'
})
export class Combatevivo {

  estado = input.required<EstadoEvento>();

  titulo = 'COMBATE EN CURSO';

}

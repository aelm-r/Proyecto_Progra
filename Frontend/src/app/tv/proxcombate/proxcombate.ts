import { Component, input } from '@angular/core';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-proxcombate',
  imports: [],
  templateUrl: './proxcombate.html',
  styleUrl: './proxcombate.css'
})
export class Proxcombate {

  estado = input.required<EstadoEvento>();

  titulo = 'PRÓXIMO COMBATE';

  mensaje = 'PREPÁRENSE PARA COMBATIR';

}

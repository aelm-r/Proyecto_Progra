import { Component, input } from '@angular/core';
import { EstadoEvento } from '../../services/torneo-api';

@Component({
  selector: 'app-bienvenida',
  imports: [],
  styleUrl: './bienvenida.css',
  templateUrl: './bienvenida.html',
})
export class Bienvenida {

  estado = input.required<EstadoEvento>();

  titulo = 'TORNEO ROBOT GALLITO';
  universidad = 'UNIVERSIDAD DE OCCIDENTE ROOSEVELT';
  evento = 'FERIA TECNOLÓGICA 2026';

}

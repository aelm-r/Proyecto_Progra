import { Component } from '@angular/core';
import { Bienvenida } from './tv/bienvenida/bienvenida';
import { Sorteo } from './tv/sorteo/sorteo';
import { Proxcombate } from './tv/proxcombate/proxcombate';
import { Combatevivo } from './tv/combatevivo/combatevivo';
import { Resultado } from './tv/resultado/resultado';
import { Rotacion } from './tv/rotacion/rotacion';
import { Campeon } from './tv/campeon/campeon';
@Component({
  selector: 'app-root',
  imports: [Bienvenida, Sorteo, Proxcombate, Combatevivo, Resultado, Rotacion, Campeon],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {

  titulo = 'TORNEO ROBOT GALLITO';

  universidad = 'UNIVERSIDAD DE OCCIDENTE ROOSEVELT';

  evento = 'FERIA TECNOLÓGICA 2026';

  mensaje = 'PREPARATE PARA EL COMBATE';

  equipos = [
  {
    nombre: 'Equipo 1',
    robot: 'Destroyer'
  },
  {
    nombre: 'Equipo 2',
    robot: 'The beast'
  },
  {
    nombre: 'Equipo 3 ',
    robot: 'Gallito X'
  },
  {
    nombre: 'Equipo 4',
    robot: 'El Empalador'
  },
  {
    nombre: 'Equipo 5',
    robot: 'Fénix'
  },
  {
    nombre: 'Equipo 6',
    robot: 'Rayo'
  },
  {
    nombre: 'Equipo 7',
    robot: 'Pacman'
  },
  {
    nombre: 'Equipo 8',
    robot: 'IT'
  }
];
estadoActual = 'rotacion'; // Estado inicial

cambiarEstado(nuevoEstado: string) {
  this.estadoActual = nuevoEstado;
}
}
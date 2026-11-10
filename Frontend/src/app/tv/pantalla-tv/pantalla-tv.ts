import { Component } from '@angular/core';
import { Bienvenida } from '../bienvenida/bienvenida';
import { Sorteo } from '../sorteo/sorteo';
import { Proxcombate } from '../proxcombate/proxcombate';
import { Combatevivo } from '../combatevivo/combatevivo';
import { Resultado } from '../resultado/resultado';
import { Rotacion } from '../rotacion/rotacion';
import { Campeon } from '../campeon/campeon';

@Component({
  selector: 'app-pantalla-tv',
  standalone: true,
  imports: [
    Bienvenida,
    Sorteo,
    Proxcombate,
    Combatevivo,
    Resultado,
    Rotacion,
    Campeon
  ],
  templateUrl: './pantalla-tv.html',
  styleUrl: './pantalla-tv.css'
})
export class PantallaTv {
  // Aquí dentro van las variables y métodos que estaban en App



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
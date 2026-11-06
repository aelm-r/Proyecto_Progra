import { Component } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-bienvenida',
  styleUrl: './bienvenida.css',
  templateUrl: './bienvenida.html',
})
export class Bienvenida {

titulo = 'TORNEO ROBOT GALLITO';
universidad = 'UNIVERSIDAD DE OCCIDENTE ROOSEVELT';
evento = 'FERIA TECNOLÓGICA 2026';

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
}

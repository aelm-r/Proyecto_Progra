
import { Component } from '@angular/core';

interface Equipo {
  nombre: string;
  robot: string;
  puntos: number;
}

@Component({
  selector: 'app-resultado',
  imports: [],
  templateUrl: './resultado.html',
  styleUrl: './resultado.css'
})
export class Resultado {

  fase = 'SEMIFINAL';
  numeroCombate = 1;

  equipoA: Equipo = {
    nombre: 'Equipo 1',
    robot: 'Destroyer',
    puntos: 2
  };

  equipoB: Equipo = {
    nombre: 'Equipo 2',
    robot: 'The beast',
    puntos: 1
  };

  ganador: Equipo = this.equipoA;

  motivoVictoria = 'Victoria por puntuación';

}


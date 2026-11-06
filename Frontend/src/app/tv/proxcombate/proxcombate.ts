import { Component, signal } from '@angular/core';

interface Equipo {
  nombre: string;
  robot: string;
  logo: string;
}

@Component({
  selector: 'app-proxcombate',
  imports: [],
  templateUrl: './proxcombate.html',
  styleUrl: './proxcombate.css'
})
export class Proxcombate {

  titulo = 'PRÓXIMO COMBATE';

  numeroCombate = signal(1);
  totalCombates = signal(16);
  fase = signal('FASE DE GRUPOS');

  equipoA = signal<Equipo>({
    nombre: 'Equipo 1',
    robot: 'Destroyer',
    logo: '🤖'
  });

  equipoB = signal<Equipo>({
    nombre: 'Equipo 2',
    robot: 'The Beast',
    logo: '🐉'
  });

  mensaje = signal('PREPÁRENSE PARA COMBATIR');

}

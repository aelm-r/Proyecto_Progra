
import { Component } from '@angular/core';

@Component({
  selector: 'app-campeon',
  imports: [],
  templateUrl: './campeon.html',
  styleUrl: './campeon.css',
})
export class Campeon {

  // Datos simulados del podio
  podio = [
    {
      posicion: 2,
      titulo: 'SEGUNDO LUGAR',
      robot: 'The Beast',
      logo: '🐉',
      color: '#C0C0C0',
      emoji: '🥈',
    },
    {
      posicion: 1,
      titulo: 'CAMPEÓN',
      robot: 'Destroyer',
      logo: '🦾',
      color: '#FFD700',
      emoji: '🥇',
    },
    {
      posicion: 3,
      titulo: 'TERCER LUGAR',
      robot: 'Gallito X',
      logo: '🐓',
      color: '#CD7F32',
      emoji: '🥉',
    },
  ];

}

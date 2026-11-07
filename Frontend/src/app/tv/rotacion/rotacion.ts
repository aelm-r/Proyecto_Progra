import { Component, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

interface PosicionEquipo {
  posicion: number;
  nombre: string;
  puntos: number;
  diferenciaAsaltos: number;
}

@Component({
  selector: 'app-rotacion',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rotacion.html',
  styleUrl: './rotacion.css'
})
export class Rotacion {

  // Fondo compartido de la arena
  imagenFondo = input<string>('assets/image_954b97.jpg');

  // Datos simulados o vinculados a la API (/api/public/sorteo, /grupos, /bracket)[cite: 6]
  grupoA = signal<PosicionEquipo[]>([
    { posicion: 1, nombre: 'Robotech', puntos: 6, diferenciaAsaltos: 4 },
    { posicion: 2, nombre: 'CyberGallos', puntos: 3, diferenciaAsaltos: 1 },
    { posicion: 3, nombre: 'Titanium', puntos: 1, diferenciaAsaltos: -2 },
    { posicion: 4, nombre: 'Voltios', puntos: 0, diferenciaAsaltos: -3 }
  ]);

  grupoB = signal<PosicionEquipo[]>([
    { posicion: 1, nombre: 'AlphaBot', puntos: 6, diferenciaAsaltos: 5 },
    { posicion: 2, nombre: 'IronChicken', puntos: 4, diferenciaAsaltos: 1 },
    { posicion: 3, nombre: 'Blaze', puntos: 1, diferenciaAsaltos: -2 },
    { posicion: 4, nombre: 'Spectre', puntos: 0, diferenciaAsaltos: -4 }
  ]);

  semifinal1 = signal('1.° A vs 2.° B');
  semifinal2 = signal('1.° B vs 2.° A');
  tercerLugar = signal('Perdedores SF');
  final = signal('Ganadores SF');
}
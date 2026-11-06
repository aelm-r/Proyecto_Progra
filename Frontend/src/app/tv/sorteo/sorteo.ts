import { Component, OnDestroy, signal } from '@angular/core';

interface Equipo {
  nombre: string;
  robot: string;
}

@Component({
  selector: 'app-sorteo',
  imports: [],
  templateUrl: './sorteo.html',
  styleUrl: './sorteo.css'
})
export class Sorteo implements OnDestroy {

  titulo = 'SORTEO DE EQUIPOS';

  grupoA = signal<Equipo[]>([]);
  grupoB = signal<Equipo[]>([]);

  equiposSorteados: Equipo[] = [
    { nombre: 'Equipo 1', robot: 'Destroyer' },
    { nombre: 'Equipo 2', robot: 'The beast' },
    { nombre: 'Equipo 3', robot: 'Gallito X' },
    { nombre: 'Equipo 4', robot: 'El Empalador' },
    { nombre: 'Equipo 5', robot: 'Fénix' },
    { nombre: 'Equipo 6', robot: 'Rayo' },
    { nombre: 'Equipo 7', robot: 'Pacman' },
    { nombre: 'Equipo 8', robot: 'IT' }
  ];

  indiceActual = 0;

  sorteoIniciado = signal(false);
  sorteoFinalizado = signal(false);
  equipoActual = signal<Equipo | null>(null);

  private temporizador?: ReturnType<typeof setInterval>;

  iniciarSorteo() {

    if (this.sorteoIniciado()) {
      return;
    }

    this.detenerTemporizador();

    this.grupoA.set([]);
    this.grupoB.set([]);

    this.indiceActual = 0;
    this.equipoActual.set(null);

    this.sorteoIniciado.set(true);
    this.sorteoFinalizado.set(false);

    this.mostrarSiguienteEquipo();

    this.temporizador = setInterval(() => {
      this.mostrarSiguienteEquipo();
    }, 2000);
  }

  mostrarSiguienteEquipo() {

    if (this.indiceActual >= this.equiposSorteados.length) {
      this.finalizarSorteo();
      return;
    }

    const equipo = this.equiposSorteados[this.indiceActual];

    this.equipoActual.set(equipo);

    if (this.indiceActual % 2 === 0) {
      this.grupoA.update(equipos => [...equipos, equipo]);
    } else {
      this.grupoB.update(equipos => [...equipos, equipo]);
    }

    this.indiceActual++;

    if (this.indiceActual >= this.equiposSorteados.length) {
      this.finalizarSorteo();
    }
  }

  finalizarSorteo() {
    this.detenerTemporizador();
    this.sorteoIniciado.set(false);
    this.sorteoFinalizado.set(true);
  }

  private detenerTemporizador() {
    if (this.temporizador !== undefined) {
      clearInterval(this.temporizador);
      this.temporizador = undefined;
    }
  }

  ngOnDestroy() {
    this.detenerTemporizador();
  }
}

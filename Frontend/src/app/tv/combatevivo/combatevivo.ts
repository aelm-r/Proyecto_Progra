import { Component, OnDestroy, OnInit, signal } from '@angular/core';

interface Equipo {
  nombre: string;
  robot: string;
  logo: string;
}

@Component({
  selector: 'app-combatevivo',
  imports: [],
  templateUrl: './combatevivo.html',
  styleUrl: './combatevivo.css'
})
export class Combatevivo implements OnInit, OnDestroy {

  titulo = 'COMBATE EN CURSO';

  // Información del combate
  numeroCombate = signal(1);
  totalCombates = signal(16);
  fase = signal('FASE DE GRUPOS');

  // Información de los equipos
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

  // Marcador de rondas ganadas
  marcadorA = signal(0);
  marcadorB = signal(0);

  // Ronda actual
  rondaActual = signal(1);
  totalRondas = 3;

  // Temporizador
  tiempoRestante = signal(90);
  temporizadorActivo = signal(false);

  // Estado de pausa técnica
  pausaTecnica = signal(false);

  // Estado de muerte súbita
  muerteSubita = signal(false);

  private intervalo?: ReturnType<typeof setInterval>;

  // Atajo de teclado para pruebas
  private manejarTecla = (evento: KeyboardEvent) => {
    if (evento.key === 'ArrowUp') {
      this.iniciarTemporizador();
    }
  };

  ngOnInit(): void {
    window.addEventListener('keydown', this.manejarTecla);
  }

  ngOnDestroy(): void {
    this.detenerTemporizador();
    window.removeEventListener('keydown', this.manejarTecla);
  }

  // Iniciar el temporizador
  iniciarTemporizador(): void {

    if (this.temporizadorActivo() || this.pausaTecnica()) {
      return;
    }

    if (this.tiempoRestante() <= 0) {
      return;
    }

    this.temporizadorActivo.set(true);

    this.intervalo = setInterval(() => {

      if (this.tiempoRestante() > 0) {
        this.tiempoRestante.update(tiempo => tiempo - 1);
      }

      if (this.tiempoRestante() === 0) {
        this.detenerTemporizador();
      }

    }, 1000);
  }

  // Detener el temporizador
  detenerTemporizador(): void {

    if (this.intervalo) {
      clearInterval(this.intervalo);
      this.intervalo = undefined;
    }

    this.temporizadorActivo.set(false);
  }

  // Activar pausa técnica
  activarPausa(): void {

    this.detenerTemporizador();
    this.pausaTecnica.set(true);
  }

  // Reanudar después de la pausa
  reanudarCombate(): void {

    this.pausaTecnica.set(false);
  }

  activarMuerteSubita(): void {
  this.detenerTemporizador();

  this.pausaTecnica.set(false);
  this.muerteSubita.set(true);
  this.tiempoRestante.set(60);
  }

  desactivarMuerteSubita(): void {
  this.detenerTemporizador();

  this.muerteSubita.set(false);
  this.pausaTecnica.set(false);
  this.tiempoRestante.set(90);
  }

}
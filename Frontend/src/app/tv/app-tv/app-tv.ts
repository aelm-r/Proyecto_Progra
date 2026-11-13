import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Subscription, timer, EMPTY } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';

import { TorneoApi, EstadoEvento } from '../../services/torneo-api';

import { Bienvenida } from '../bienvenida/bienvenida';
import { Sorteo } from '../sorteo/sorteo';
import { Proxcombate } from '../proxcombate/proxcombate';
import { Combatevivo } from '../combatevivo/combatevivo';
import { Resultado } from '../resultado/resultado';
import { Rotacion } from '../rotacion/rotacion';
import { Campeon } from '../campeon/campeon';
import { Eliminatoria } from '../eliminatoria/eliminatoria';

@Component({
  selector: 'app-tv',
  imports: [
    Bienvenida,
    Sorteo,
    Proxcombate,
    Combatevivo,
    Resultado,
    Rotacion,
    Campeon,
    Eliminatoria
  ],
  templateUrl: './app-tv.html',
  styleUrl: './app-tv.css'
})
export class AppTv implements OnInit, OnDestroy {

  private torneoApi = inject(TorneoApi);
  private suscripcion?: Subscription;

  titulo = 'TORNEO ROBOT GALLITO';
  universidad = 'UNIVERSIDAD DE OCCIDENTE ROOSEVELT';
  evento = 'FERIA TECNOLÓGICA 2026';
  mensaje = 'PREPARATE PARA EL COMBATE';

  // Último estado recibido desde el backend.
  estadoEvento: EstadoEvento | null = null;

  // Pantalla que se mostrará actualmente.
  estadoActual = 'bienvenida';

  ngOnInit(): void {
    this.suscripcion = timer(0, 2000).pipe(
      switchMap(() =>
        this.torneoApi.obtenerEstadoEvento().pipe(
          catchError(error => {
            console.error('Error al consultar el estado del torneo:', error);
            return EMPTY;
          })
        )
      )
    ).subscribe(estado => {
      this.estadoEvento = estado;
      this.actualizarPantalla(estado);
    });
  }

  ngOnDestroy(): void {
    this.suscripcion?.unsubscribe();
  }

  private actualizarPantalla(estado: EstadoEvento): void {

    const fase = estado.fase.toLowerCase();

    // El torneo todavía está en preparación.
    if (fase === 'preparacion') {
      this.estadoActual = 'bienvenida';
      return;
    }

    // Mostrar el sorteo cuando el backend indique que ya se realizó.
    if (fase === 'sorteo_realizado') {
      this.estadoActual = 'sorteo';
      return;
    }

    // Si hay un combate activo, mostrarlo.
    if (estado.combateEnCurso !== null) {
      this.estadoActual = 'combatevivo';
      return;
    }

    // Mostrar el último resultado durante 15 segundos.
    if (estado.ultimoFinalizado?.fechaFin) {
      const horaServidor = new Date(estado.horaServidor).getTime();
      const horaFinalizacion = new Date(
        estado.ultimoFinalizado.fechaFin
      ).getTime();

      const segundosDesdeFinalizacion =
        horaServidor - horaFinalizacion;

      if (
        segundosDesdeFinalizacion >= 0 &&
        segundosDesdeFinalizacion <= 15000
      ) {
        this.estadoActual = 'resultado';
        return;
      }
    }

    // Mostrar el siguiente combate si está programado.
    if (estado.siguienteCombate !== null) {
      this.estadoActual = 'proxcombate';
      return;
    }

    // Si no hay otro combate, mostrar la información de la fase.
    if (fase === 'grupos') {
      this.estadoActual = 'rotacion';
      return;
    }

    if (fase === 'eliminatoria') {
      this.estadoActual = 'eliminatoria';
      return;
    }

    if (fase === 'finalizado') {
      this.estadoActual = 'campeon';
      return;
    }

    this.estadoActual = 'bienvenida';
  }

  cambiarEstado(nuevoEstado: string): void {
    this.estadoActual = nuevoEstado;
  }
}

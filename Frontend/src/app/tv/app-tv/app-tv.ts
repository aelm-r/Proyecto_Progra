import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { Subscription, timer, EMPTY } from 'rxjs';
import { exhaustMap, catchError } from 'rxjs/operators';

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
  estadoEvento = signal<EstadoEvento | null>(null);

  // Pantalla indicada por el backend.
  estadoActual = signal('bienvenida');

  ngOnInit(): void {
    this.suscripcion = timer(0, 2000).pipe(
      exhaustMap(() =>
        this.torneoApi.obtenerEstadoEvento().pipe(
          catchError(error => {
            console.error('Error al consultar el estado del torneo:', error);
            return EMPTY;
          })
        )
      )
    ).subscribe(estado => {
      this.estadoEvento.set(estado);
      this.estadoActual.set(estado.pantallaActual);
    });
  }

  ngOnDestroy(): void {
    this.suscripcion?.unsubscribe();
  }
}
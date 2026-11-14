import { Component } from '@angular/core';
import { PanelJuez } from './panel-juez/panel-juez';

// Contenedor de la sección /juez (la ruta ya existe en app.routes.ts, no hay que tocarla).
@Component({
  selector: 'app-juez',
  imports: [PanelJuez],
  template: `<app-panel-juez />`,
})
export class AppJuez {}

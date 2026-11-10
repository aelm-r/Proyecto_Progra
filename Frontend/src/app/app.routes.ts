import { Routes } from '@angular/router';
import { PanelJuez } from './juez/panel-juez/panel-juez';
import { PantallaTv } from './tv/pantalla-tv/pantalla-tv';

export const routes: Routes = [
  { path: '', component: PantallaTv },      // http://localhost:4200/ (Pantalla TV)
  { path: 'juez', component: PanelJuez },   // http://localhost:4200/juez (Panel Juez)
  { path: '**', redirectTo: '' }            // Redirige cualquier ruta inválida a la TV
];

import { Routes } from '@angular/router';

import { AppTv } from './tv/app-tv/app-tv';
import { AppJuez } from './juez/app-juez';
import { AdminComponent } from './admin/admin.component';
import { EquiposComponent } from './admin/equipos.component';
import { SorteoComponent } from './admin/sorteo.component';
import { CorreccionesComponent } from './admin/correcciones.component';
import { LoginComponent } from './login/login.component';
import { rolGuard } from './core/rol.guard';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'tv',
    pathMatch: 'full'
  },

  {
    path: 'tv',
    component: AppTv
  },

  {
    path: 'juez',
    component: AppJuez
  },

  {
    path: 'login',
    component: LoginComponent
  },

  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [rolGuard('Admin')],
    children: [
      { path: '', redirectTo: 'equipos', pathMatch: 'full' },
      { path: 'equipos', component: EquiposComponent },
      { path: 'sorteo', component: SorteoComponent },
      { path: 'correcciones', component: CorreccionesComponent }
    ]
  },

  {
    path: '**',
    redirectTo: 'tv'
  }

];

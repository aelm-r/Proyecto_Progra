import { Routes } from '@angular/router';
import { AdminComponent } from './admin/admin.component';
import { CorreccionesComponent } from './admin/correcciones.component';
import { EquiposComponent } from './admin/equipos.component';
import { SorteoComponent } from './admin/sorteo.component';
import { REQUIERE_LOGIN } from './core/config';
import { rolGuard } from './core/rol.guard';
import { LoginComponent } from './login/login.component';

export const routes: Routes = [
  { path: '', redirectTo: REQUIERE_LOGIN ? 'login' : 'admin', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: REQUIERE_LOGIN ? [rolGuard('Admin')] : [],
    children: [
      { path: 'equipos', component: EquiposComponent },
      { path: 'sorteo', component: SorteoComponent },
      { path: 'correcciones', component: CorreccionesComponent },
      { path: '', redirectTo: 'equipos', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: REQUIERE_LOGIN ? 'login' : 'admin' }
];
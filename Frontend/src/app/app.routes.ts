import { Routes } from '@angular/router';

import { AppTv } from './tv/app-tv/app-tv';
import { AppJuez } from './juez/app-juez';
import { AppAdmin } from './admin/app-admin';

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
    path: 'admin',
    component: AppAdmin
  },

  {
    path: '**',
    redirectTo: 'tv'
  }

];

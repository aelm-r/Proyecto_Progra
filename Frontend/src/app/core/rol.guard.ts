import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const rolGuard = (rol: string): CanActivateFn => () => {
  const auth = inject(AuthService);
  return auth.rol() === rol ? true : inject(Router).parseUrl('/login');
};

import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
// dentro de providers:


export const appConfig: ApplicationConfig = {
  providers: [ 
    provideHttpClient(),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes)
  ]
};

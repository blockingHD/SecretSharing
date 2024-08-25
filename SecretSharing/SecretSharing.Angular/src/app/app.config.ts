import {ApplicationConfig, provideZoneChangeDetection} from '@angular/core';
import {provideHttpClient} from '@angular/common/http';
import {provideRouter} from '@angular/router';

import {routes} from './app.routes';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {provideAuth0} from "@auth0/auth0-angular";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({eventCoalescing: true}),
    provideRouter(routes),
    provideHttpClient(),
    provideAnimationsAsync(),
    provideAuth0({
      domain: 'dev-kbivp6b1lxnmfboj.uk.auth0.com',
      clientId: 'ubAbwHFgdgbafNpPVLreMyfwiaI0VNtI',
      authorizationParams: {
        redirect_uri: window.location.origin
      }
    })
  ]
};

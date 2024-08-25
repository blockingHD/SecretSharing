import { Routes } from '@angular/router';
import {AuthGuard} from "@auth0/auth0-angular";

export const routes: Routes = [
  {
    path: 'my-secrets',
    canActivate: [AuthGuard],
    loadComponent: () => import('./my-secrets/my-secrets.component').then(m => m.MySecretsComponent)
  },
  {
    path: 'send-secret',
    canActivate: [AuthGuard],
    loadComponent: () => import('./send-secret/send-secret.component').then(m => m.SendSecretComponent)
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'my-secrets'
  }
];

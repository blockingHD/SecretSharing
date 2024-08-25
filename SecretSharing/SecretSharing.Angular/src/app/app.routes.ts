import { Routes } from '@angular/router';
import {MsalGuard} from "@azure/msal-angular";

export const routes: Routes = [
  {
    path: 'my-secrets',
    canActivate: [MsalGuard],
    loadComponent: () => import('./my-secrets/my-secrets.component').then(m => m.MySecretsComponent)
  },
  {
    path: 'send-secret',
    canActivate: [MsalGuard],
    loadComponent: () => import('./send-secret/send-secret.component').then(m => m.SendSecretComponent)
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'my-secrets'
  }
];

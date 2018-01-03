import { Routes } from '@angular/router';

import * as pages from './pages';

export const rootRouterConfig: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: pages.HomePageComponent },
  { path: 'log', component: pages.LogViewPageComponent }
];


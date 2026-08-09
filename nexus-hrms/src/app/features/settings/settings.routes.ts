import { Routes } from "@angular/router";
import { SecuritySettingsComponent } from "./pages/security-settings/security-settings";

export const SETTINGS_ROUTES: Routes = [
  {
    path: 'security',
    component: SecuritySettingsComponent,
    title: 'Security Settings - Nexus HRMS'
  },
  {
    path: '',
    redirectTo: 'security',
    pathMatch: 'full'
  }
];
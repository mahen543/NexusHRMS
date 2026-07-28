import { Routes } from "@angular/router";
import { LoginComponent } from "./pages/login/login";

export const AUTH_ROUTES: Routes = [
{
    path: '',
    redirectTo: 'login',
    pathMatch: 'full' // Redirects /auth directly to /auth/login
  },
  {
    path: 'login',
    component: LoginComponent
  }
];
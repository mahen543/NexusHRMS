import { Routes } from "@angular/router";
import { EmployeeDashboard } from "./pages/employee-dashboard/employee-dashboard";

export const DASHBOARD_ROUTES: Routes = [
  {
    path: 'employee_dashboard',
    component: EmployeeDashboard
  }
]
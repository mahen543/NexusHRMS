import { Routes } from "@angular/router";
import { AttendanceReportsComponent } from "../reports/pages/attendance-reports/attendance-reports";

export const REPORTS_ROUTES: Routes = [
  {
    path: 'reports',
    component: AttendanceReportsComponent,
    title: 'reports - Nexus HRMS'
  },
  {
    path: '',
    redirectTo: 'reports',
    pathMatch: 'full'
  }
];
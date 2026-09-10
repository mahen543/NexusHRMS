import { Routes } from "@angular/router";
import { AttendanceTableComponent } from "./components/attendance-table/attendance-table";

export const ATTENDANCE_ROUTES: Routes = [
  {
    path: 'employee_attandance',
    component: AttendanceTableComponent
  }
]
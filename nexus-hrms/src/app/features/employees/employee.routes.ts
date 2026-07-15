import { Routes } from '@angular/router';

import { EmployeeListComponent } from './pages/employee-list/employee-list';



export const EMPLOYEE_ROUTES: Routes = [
  {
    path: 'list',
    component: EmployeeListComponent
  }
]

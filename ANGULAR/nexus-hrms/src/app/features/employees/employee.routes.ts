import { Routes } from '@angular/router';

import { EmployeeListComponent } from './pages/employee-list/employee-list';
import { EmployeeCreateComponent } from './pages/employee-create/employee-create';
import { EmployeeEditComponent } from './pages/employee-edit/employee-edit';



export const EMPLOYEE_ROUTES: Routes = [
  {
    path: '',
    component: EmployeeListComponent
  },
  {
    path: 'create',
    component: EmployeeCreateComponent,
  },
  {
    path: 'edit/:id',
    component: EmployeeEditComponent,
  }
]

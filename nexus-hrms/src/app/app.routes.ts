import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout';

export const routes: Routes = [

    {
    path: '',
    component: AdminLayoutComponent,
    children: [

      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes')
            .then(m => m.DASHBOARD_ROUTES)
      },

      {
        path: 'employees',
        loadChildren: () =>
          import('./features/employees/employee.routes')
            .then(m => m.EMPLOYEE_ROUTES)
      },

      {
        path: 'attendance',
        loadChildren: () =>
          import('./features/attendance/attendance.routes')
            .then(m => m.ATTENDANCE_ROUTES)
      }

    ]
  },

  {
    path: 'auth',
    component: AuthLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () =>
          import('./features/auth/auth.routes')
            .then(m => m.AUTH_ROUTES)
      }
    ]
  }
];

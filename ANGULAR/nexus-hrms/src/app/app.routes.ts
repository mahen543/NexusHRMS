import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  // 1. Auth routes (PUBLIC - NO GUARD)
  {
    path: 'auth',
    component: AuthLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () =>
          import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
      }
    ]
  },

  // 2. Protected Admin App Routes (GUARDED)
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES)
      },
      {
        path: 'employees',
        loadChildren: () =>
          import('./features/employees/employee.routes').then(m => m.EMPLOYEE_ROUTES)
      },
      {
        path: 'change-password',
        loadComponent: () =>
          import('./features/auth/pages/change-password/change-password')
            .then(m => m.ChangePasswordComponent)
      },
      {
        path: 'attendance',
        loadChildren: () =>
          import('./features/attendance/attendance.routes').then(m => m.ATTENDANCE_ROUTES)
      },
      {
        path: 'appraisals',
        loadChildren: () =>
          import('./features/appraisals/appraisals.routes').then(m => m.APPRAISALS_ROUTES)
      },
      {
        path: 'approvals',
        loadChildren: () =>
          import('./features/approvals/approvals.routes').then(m => m.APPROVALS_ROUTES)
      },
      {
        path: 'masters',
        loadChildren: () =>
          import('./features/masters/masters.routes').then(m => m.MASTERS_ROUTES)
      },
      {
        path: 'payroll',
        loadChildren: () =>
          import('./features/payroll/payroll.routes').then(m => m.PAYROLL_ROUTES)
      },
      {
        path: 'projects',
        loadChildren: () =>
          import('./features/projects/projects.routes').then(m => m.PROJECTS_ROUTES)
      },
      {
        path: 'reports',
        loadChildren: () =>
          import('./features/reports/reports.routes').then(m => m.REPORTS_ROUTES)
      },
      {
        path: 'settings',
        loadChildren: () =>
          import('./features/settings/settings.routes').then(m => m.SETTINGS_ROUTES)
      }
    ]
  },

  // 3. Fallback redirect
  {
    path: '**',
    redirectTo: 'auth/login'
  }
];
import { Routes } from "@angular/router";
import { SalaryHistoryComponent } from "./pages/salary-history/salary-history";

export const PAYROLL_ROUTES: Routes = [
  {
    path: 'salary_history',
    component: SalaryHistoryComponent
  }
]
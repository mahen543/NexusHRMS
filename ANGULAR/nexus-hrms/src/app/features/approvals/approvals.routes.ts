import { Routes } from "@angular/router";
import { ApprovalListComponent } from "./pages/approval-list/approval-list";

export const APPROVALS_ROUTES: Routes = [
  {
    path: 'approval_list',
    component: ApprovalListComponent
  }
]
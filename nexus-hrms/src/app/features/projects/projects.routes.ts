import { Routes } from "@angular/router";
import { ProjectListComponent } from "./pages/project-list/project-list";

export const PROJECTS_ROUTES: Routes = [
  {
    path: 'project_list',
    component: ProjectListComponent
  }
]
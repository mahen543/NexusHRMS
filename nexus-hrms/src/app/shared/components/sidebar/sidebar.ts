import { Component, Input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [ RouterLink, RouterLinkActive ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class SidebarComponent {
  @Input() collapsed = false;

  menus = [
    {
      label: 'Dashboard',
      route: '/dashboard',
      icon: '📊',
    },
    {
      label: 'Employees',
      route: '/employees',
      icon: '👨‍💼',
    },
    {
      label: 'Attendance',
      route: '/attendance',
      icon: '🕒',
    },
    {
      label: 'Appraisals',
      route: '/appraisals',
      icon: '⭐',
    },
    {
      label: 'Payroll',
      route: '/payroll',
      icon: '💰',
    },
    {
      label: 'Projects',
      route: '/projects',
      icon: '📁',
    },
    {
      label: 'Approvals',
      route: '/approvals',
      icon: '✔️',
    },
    {
      label: 'Reports',
      route: '/reports',
      icon: '📈',
    },
    {
      label: 'Masters',
      route: '/masters',
      icon: '⚙️',
    },
    {
      label: 'Settings',
      route: '/settings',
      icon: '🔧',
    }
  ];
}

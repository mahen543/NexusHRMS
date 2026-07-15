import { Component } from '@angular/core';
import { SidebarComponent } from "../../shared/components/sidebar/sidebar";
import { NavbarComponent } from "../../shared/components/navbar/navbar";
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from '../../shared/components/footer/footer';

@Component({
  selector: 'app-admin-layout',
  imports: [ RouterOutlet,
    NavbarComponent,
    SidebarComponent,
    FooterComponent ],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
})
export class AdminLayoutComponent {
    collapsed = false;

  toggleSidebar(): void {
    this.collapsed = !this.collapsed;
  }
}

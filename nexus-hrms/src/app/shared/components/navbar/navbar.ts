import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-navbar',
  imports: [],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class NavbarComponent {
  
  @Output() toggleSidebar = new EventEmitter<void>();

  notificationCount = 5;

  onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }
}

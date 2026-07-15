import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-status-badge',
  imports: [ CommonModule],
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.css',
})
export class StatusBadgeComponent {
    @Input() status: 'active' | 'inactive' | 'pending' | 'approved' | 'rejected' = 'pending';
}

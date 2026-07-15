import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-page-header',
  imports: [ CommonModule ],
  templateUrl: './page-header.html',
  styleUrl: './page-header.css',
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() subtitle = '';

  @Input() showAdd = false;
  @Input() addLabel = 'Add';

  @Output() addClick = new EventEmitter<void>();

  onAdd(): void {
    this.addClick.emit();
  }
}

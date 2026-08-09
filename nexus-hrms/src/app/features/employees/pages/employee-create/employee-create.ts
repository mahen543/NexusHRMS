import { Component, inject, signal } from '@angular/core';
import { CreateEmployeeRequest } from '../../models/employee.model';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';

@Component({
  selector: 'app-employee-create',
  imports: [ ReactiveFormsModule ],
  templateUrl: './employee-create.html',
  styleUrl: './employee-create.css',
})
export class EmployeeCreateComponent {
  private fb = inject(FormBuilder);
  private employeeService = inject(EmployeeService);
  private router = inject(Router);

  isSubmitting = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  employeeForm = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    designationId: [1, [Validators.required, Validators.min(1)]],
    hierarchyLevel: [1, [Validators.required, Validators.min(1)]]
  });

  isFieldInvalid(fieldName: string): boolean {
    const field = this.employeeForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.successMessage.set(null);
    this.errorMessage.set(null);

    const payload: CreateEmployeeRequest = {
      firstName: this.employeeForm.value.firstName || '',
      lastName: this.employeeForm.value.lastName || '',
      email: this.employeeForm.value.email || '',
      designationId: Number(this.employeeForm.value.designationId),
      hierarchyLevel: Number(this.employeeForm.value.hierarchyLevel)
    };

    this.employeeService.createEmployee(payload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.successMessage.set('Employee created successfully!');
        this.employeeForm.reset({ designationId: 1, hierarchyLevel: 1 });
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to create employee.');
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/employees']);
  }
}

import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../../../core/services/auth.service';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ChangePasswordRequest } from '../../../../core/models/auth.model';

@Component({
  selector: 'app-security-settings',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './security-settings.html',
  styleUrl: './security-settings.css',
})
export class SecuritySettingsComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  isSubmitting = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  passwordForm = this.fb.group(
    {
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    },
    { validators: this.passwordMatchValidator }
  );

  private passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const newPass = group.get('newPassword')?.value;
    const confirmPass = group.get('confirmPassword')?.value;

    if (!newPass || !confirmPass) {
      return null;
    }

    return newPass === confirmPass ? null : { mismatch: true };
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.passwordForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      this.errorMessage.set('User is not authenticated.');
      return;
    }

    this.isSubmitting.set(true);
    this.successMessage.set(null);
    this.errorMessage.set(null);

    // Map UI inputs to match API payload signature
    const payload: ChangePasswordRequest = {
      userId: currentUser.id,
      currentPassword: this.passwordForm.value.currentPassword || '',
      newPassword: this.passwordForm.value.newPassword || '',
      ipAddress: '127.0.0.1' // Can be replaced with dynamic client IP resolution
    };

    this.authService.changePassword(payload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.successMessage.set('Password updated successfully!');
        this.passwordForm.reset();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(
          err?.error?.message || 'Failed to update password. Please verify current details.'
        );
      }
    });
  }
}
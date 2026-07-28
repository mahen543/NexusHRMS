// login.ts
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  loginForm: FormGroup = this.fb.group({
    email: ['admin', [Validators.required]],
    password: ['admin123', [Validators.required, Validators.minLength(6)]]
  });

  isSubmitted = false;
  isLoading = false;
  showPassword = false;
  errorMessage = '';
  successMessage = '';

  get f() {
    return this.loginForm.controls;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    this.isSubmitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;

    const { email, password } = this.loginForm.value;

    // Call raw API method from AuthService
    this.authService.login(email, password).subscribe({
      next: (tokenResponse) => {
        this.isLoading = false;
        this.successMessage = 'Login successful! Redirecting...';

        // Save token to localStorage
        this.authService.setToken(tokenResponse);

        // Redirect to employees page
        setTimeout(() => {
          this.router.navigate(['/employees']);
        }, 800);
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Login error:', err);
        
        if (err.status === 401 || err.status === 400) {
          this.errorMessage = 'Invalid username or password.';
        } else {
          this.errorMessage = 'Unable to connect to auth server.';
        }
      }
    });
  }
}
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
  ) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    const { email, password } = this.registerForm.value;

    this.authService.register(email, password).subscribe({
      next: () => {
        this.successMessage =
          'Registration successful! Redirecting to login...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1000);
      },
      error: (error) => {
        console.error('Registration failed:', error);
        this.errorMessage =
          error.error || 'Registration failed. Please try again.';
      },
    });
  }
}

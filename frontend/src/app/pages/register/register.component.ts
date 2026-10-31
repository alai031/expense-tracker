import { Component, OnDestroy } from '@angular/core';
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
export class RegisterComponent implements OnDestroy {
  registerForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  isSubmitting = false;
  showSlowHint = false;
  private slowHintTimer?: ReturnType<typeof setTimeout>;

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
    this.startSubmitting();

    const { email, password } = this.registerForm.value;

    this.authService.register(email, password).subscribe({
      next: () => {
        // Stay disabled through the redirect so the account isn't submitted twice.
        this.showSlowHint = false;
        clearTimeout(this.slowHintTimer);
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
        this.stopSubmitting();
      },
    });
  }

  ngOnDestroy(): void {
    clearTimeout(this.slowHintTimer);
  }

  // The hosted API can take up to a minute to wake from sleep.
  private startSubmitting(): void {
    this.isSubmitting = true;
    this.slowHintTimer = setTimeout(() => (this.showSlowHint = true), 5000);
  }

  private stopSubmitting(): void {
    this.isSubmitting = false;
    this.showSlowHint = false;
    clearTimeout(this.slowHintTimer);
  }
}

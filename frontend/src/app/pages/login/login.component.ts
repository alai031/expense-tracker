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
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnDestroy {
  loginForm: FormGroup;
  errorMessage = '';
  isSubmitting = false;
  showSlowHint = false;
  private slowHintTimer?: ReturnType<typeof setTimeout>;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.startSubmitting();

    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe({
      next: (response) => {
        localStorage.setItem('token', response.token);
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        console.error('Login failed:', error);
        this.errorMessage = error.error || 'Invalid email or password.';
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

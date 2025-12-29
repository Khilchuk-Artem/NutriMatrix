import { Component } from '@angular/core';
import {AuthService} from '../Services/auth.service';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {NgClass, NgIf} from '@angular/common';

@Component({
  selector: 'app-password-recovery',
  imports: [
    FormsModule,
    NgIf,
    ReactiveFormsModule,
    NgClass
  ],
  templateUrl: './password-recovery.component.html',
  standalone: true,
  styleUrl: './password-recovery.component.css'
})
export class PasswordRecoveryComponent {
  form: FormGroup;
  successMessage = '';
  errorMessage = '';

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  get emailControl() {
    return this.form.get('email');
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const email = this.emailControl!.value;
    this.authService.requestResetPassword(email).subscribe({
      next: () => {
        this.successMessage = 'Link for password reset has been sent to your email.';
      },
      error: (err) => {
        this.errorMessage = 'An error occurred. Please try again.';
        console.error(err);
      },
    });
  }
}

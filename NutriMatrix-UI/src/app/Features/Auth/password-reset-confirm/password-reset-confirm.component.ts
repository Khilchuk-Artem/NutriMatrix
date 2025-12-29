import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../Services/auth.service';
import {ResetPasswordDTO} from '../Models/ResetPasswordDTO';
import {NgClass, NgIf} from '@angular/common';

@Component({
  selector: 'app-password-reset-confirm',
  imports: [
    ReactiveFormsModule,
    NgClass,
    NgIf
  ],
  templateUrl: './password-reset-confirm.component.html',
  standalone: true,
  styleUrl: './password-reset-confirm.component.css'
})
export class PasswordResetConfirmComponent implements OnInit {
  resetForm!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  token!: string;
  email!: string;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'] || '';
      this.email = params['email'] || '';

      if (!this.token || !this.email) {
        this.errorMessage = 'Invalid password reset link.';
      }
    });

    this.resetForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', [Validators.required]],
    }, { validator: this.passwordMatchValidator });
  }

  get newPassword() {
    return this.resetForm.get('newPassword');
  }
  get confirmNewPassword() {
    return this.resetForm.get('confirmNewPassword');
  }

  passwordMatchValidator(group: FormGroup) {
    const pass = group.get('newPassword')?.value;
    const confirmPass = group.get('confirmNewPassword')?.value;
    return pass === confirmPass ? null : { notMatching: true };
  }

  onSubmit() {
    this.errorMessage = null;
    this.successMessage = null;

    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    const dto: ResetPasswordDTO = {
      email: this.email,
      token: this.token,
      newPassword: this.newPassword?.value,
    };

    this.authService.resetPassword(dto).subscribe({
      next: (msg) => {
        this.successMessage = msg || 'Password has been reset successfully!';
        setTimeout(() => {
          this.authService.logout()
          this.router.navigate(['/auth/login']);
        }, 3000);
      },
      error: (err) => {
        this.errorMessage = err?.error || 'Failed to reset password. Please try again.';
      }
    });
  }
}

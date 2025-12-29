import { Component } from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {AuthService} from '../Services/auth.service';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';
import {NgClass, NgIf} from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [
    NgClass,
    ReactiveFormsModule,
    NgIf
  ],
  templateUrl: './register.component.html',
  standalone: true,
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage: string | null = null;
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  get name() {
    return this.registerForm.get('name');
  }

  get email() {
    return this.registerForm.get('email');
  }

  get password() {
    return this.registerForm.get('password');
  }

  onSubmit(): void {

    const { name, email, password } = this.registerForm.value;

    this.authService
      .register({ name, email, password, roles: ["User"] })
      .subscribe({
        next: (res) => {
          this.toastr.success(res.message || 'Registration successful');
          this.router.navigate(['/auth/login']);
        },
        error: (err) => {
          this.errorMessage = err?.error?.message || 'Registration failed. Please try again.';
        },
      });
  }
}

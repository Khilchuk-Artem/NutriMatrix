import { Component } from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {AuthService} from '../Services/auth.service';
import {NgClass, NgIf} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {CookieService} from 'ngx-cookie-service';
import {Router} from '@angular/router';
import {AngularFireAuth, AngularFireAuthModule} from '@angular/fire/compat/auth';
import firebase from 'firebase/compat/app';
import {AngularFireModule} from '@angular/fire/compat';
import {environment} from '../../../../environments/environment.development';
import {Auth, signInWithPopup} from '@angular/fire/auth';
import { GoogleAuthProvider } from '@angular/fire/auth';
declare const google: any;

@Component({
  selector: 'app-login',
  imports: [
    NgClass,
    ReactiveFormsModule,
    NgIf,
  ],
  templateUrl: './login.component.html',
  standalone: true,
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder,
              private authService: AuthService,
              private toastr:ToastrService,
              private cookieService:CookieService,
              private auth: Auth, // Changed from AngularFireAuth to Auth
              private router:Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const { email, password } = this.loginForm.value;
    this.authService.login({email, password}).subscribe({
      next: (res) => {
        console.log(res)

        this.toastr.success("Successfully logged in")
        this.cookieService.set('Authorization', `Bearer ${res.token}`,
          undefined,'/',undefined,true,"Strict");
        this.authService.setUser(res)

        this.router.navigate(['']);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Login failed. Please try again.';
      },
    });

  }
  async onGoogleSignIn(): Promise<void> {
    try {
      const provider = new GoogleAuthProvider();
      const result = await signInWithPopup(this.auth, provider);
      if (result.user) {
        const idToken = await result.user.getIdToken(); // Get the ID token
        this.handleGoogleLogin(idToken);
      }
    } catch (error) {
      console.error('Google Sign-In error:', error);
    }
  }

  private handleGoogleLogin(idToken: string): void {
    this.authService.loginViaGoogle(idToken).subscribe({
      next: (res) => {
        this.toastr.success('Successfully logged in with Google');
        this.cookieService.set('Authorization', `Bearer ${res.token}`, undefined, '/', undefined, true, 'Strict');
        this.authService.setUser(res);
        this.router.navigate(['']);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Google login failed. Please try again.';
      },
    });
  }
}

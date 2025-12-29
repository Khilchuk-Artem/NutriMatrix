import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '../Services/auth.service';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-confirm-email',
  imports: [],
  templateUrl: './confirm-email.component.html',
  standalone: true,
  styleUrl: './confirm-email.component.css'
})
export class ConfirmEmailComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      const email = params['email'];

      if (token && email) {
        this.authService.confirmEmail({ token, email }).subscribe({
          next: () => {
            this.toastr.success('Email confirmed successfully! You can now log in.');
            this.router.navigate(['/auth/login']);
          },
          error: () => {
            this.toastr.error('Email confirmation failed. Please try again or contact support.');
            this.router.navigate(['/auth/login']);
          }
        });
      } else {
        this.toastr.warning('Missing confirmation parameters.');
        this.router.navigate(['/auth/login']);
      }
    });
  }
}

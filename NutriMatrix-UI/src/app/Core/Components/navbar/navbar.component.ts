import {Component, OnInit} from '@angular/core';
import {AuthService} from '../../../Features/Auth/Services/auth.service';
import {Router, RouterLink} from '@angular/router';
import {UserDto} from '../../../Features/Auth/Models/UserDTO';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-navbar',
  imports: [
    NgIf,
    RouterLink
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  standalone: true,
})
export class NavbarComponent implements OnInit{
  user?: UserDto;

  constructor(private authService: AuthService,
              private router: Router) {
  }
  ngOnInit(): void {
    this.authService.user()
      .subscribe({
        next: (response) => {
          this.user = response;
        }
      });

    this.user = this.authService.getUser();
  }

  logout() {
    this.authService.logout();
  }
}

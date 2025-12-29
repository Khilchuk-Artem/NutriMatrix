import { Component } from '@angular/core';
import {RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';
import {AuthService} from '../../../Features/Auth/Services/auth.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterLinkActive,
    RouterLink,
    RouterOutlet,
    NgIf
  ],
  templateUrl: './sidebar.component.html',
  standalone: true,
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent{
  submenuOpen: Record<string, boolean> = {
    diary: false,
    trends: false,
  };
  constructor(private authService:AuthService) {
  }
  toggleSubmenu(menu: 'diary' | 'trends') {
    this.submenuOpen[menu] = !this.submenuOpen[menu];
  }
  isAdmin(){
    return this.authService.getUser()?.roles.includes('Admin');
  }
}

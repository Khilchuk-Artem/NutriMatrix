import { Component } from '@angular/core';
import {RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterLinkActive,
    RouterLink,
    RouterOutlet
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

  toggleSubmenu(menu: 'diary' | 'trends') {
    this.submenuOpen[menu] = !this.submenuOpen[menu];
  }
}

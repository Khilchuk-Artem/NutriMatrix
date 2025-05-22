import { Routes } from '@angular/router';
import {HomepageComponent} from './Features/homepage/homepage.component';
import {LoginComponent} from './Features/Auth/login/login.component';
import {PasswordRecoveryComponent} from './Features/Auth/password-recovery/password-recovery.component';
import {ConfirmEmailComponent} from './Features/Auth/confirm-email/confirm-email.component';
import {RegisterComponent} from './Features/Auth/register/register.component';
import {PasswordResetConfirmComponent} from './Features/Auth/password-reset-confirm/password-reset-confirm.component';
import {SidebarComponent} from './Core/Components/sidebar/sidebar.component';
import {DashboardComponent} from './Features/dashboard/dashboard.component';
import {ViewUserComponent} from './Features/Auth/view-user/view-user.component';
import {EditUserComponent} from './Features/Auth/edit-user/edit-user.component';

export const routes: Routes = [
  {
    component:HomepageComponent,
    path:''
  },
  {
    component:LoginComponent,
    path:'auth/login'
  },
  {
    component:PasswordRecoveryComponent,
    path:'auth/password-recovery'
  },
  {
    path: 'auth/confirm-email',
    component: ConfirmEmailComponent
  },
  {
    path: 'auth/register',
    component: RegisterComponent
  },
  {
    path: 'auth/reset-password/confirm',
    component: PasswordResetConfirmComponent
  },
  {
    path: 'app',
    component: SidebarComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path:'me', component:ViewUserComponent },
      { path:'me/edit', component:EditUserComponent }

    ],
  }
];

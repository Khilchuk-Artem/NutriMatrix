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
import {
  ViewPrivateRecipesComponent
} from './Features/FoodCatalog/Meals/view-private-recipes/view-private-recipes.component';
import {
  ViewPrivateRecipeComponent
} from './Features/FoodCatalog/Meals/view-private-recipe/view-private-recipe.component';
import {AddPrivateRecipeComponent} from './Features/FoodCatalog/Meals/add-private-recipe/add-private-recipe.component';
import {
  EditPrivateRecipeComponent
} from './Features/FoodCatalog/Meals/edit-private-recipe/edit-private-recipe.component';
import {GenerateReportComponent} from './Features/FoodRecords/generate-report/generate-report.component';
import {ViewPublicRecipesComponent} from './Features/Recipes/view-public-recipes/view-public-recipes.component';
import {GetRecommendationComponent} from './Features/Recomendation/get-recommendation/get-recommendation.component';
import {ViewSchedulesComponent} from './Features/Scheduling/view-schedules/view-schedules.component';
import {CreateSchedulingComponent} from './Features/Scheduling/create-scheduling/create-scheduling.component';
import {ViewPublicRecipeComponent} from './Features/Recipes/view-public-recipe/view-public-recipe.component';
import {AddPublicRecipeComponent} from './Features/Recipes/add-public-recipe/add-public-recipe.component';
import {EditPublicRecipeComponent} from './Features/Recipes/edit-public-recipe/edit-public-recipe.component';
import {
  NutrientConsumptionStatisticsComponent
} from './Features/FoodRecords/nutrient-consumption-statistics/nutrient-consumption-statistics.component';

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
      { path:'me/edit', component:EditUserComponent },
      { path:'recipes/private', component:ViewPrivateRecipesComponent},
      { path:'recipes/private/add', component:AddPrivateRecipeComponent},
      { path:'recipes/private/:id', component:ViewPrivateRecipeComponent},
      { path:'recipes/private/:id/edit', component:EditPrivateRecipeComponent},
      { path:'generate-report', component:GenerateReportComponent},
      { path:'recipes/public', component:ViewPublicRecipesComponent},
      { path:'recipes/public/add', component:AddPublicRecipeComponent},
      { path:'recipes/public/:id', component:ViewPublicRecipeComponent},
      { path:'recipes/public/:id/edit', component:EditPublicRecipeComponent},
      { path:'recommendation', component:GetRecommendationComponent},
      { path:'scheduling', component:ViewSchedulesComponent},
      { path:'scheduling/add', component:CreateSchedulingComponent},
      { path:'consumption-stats', component:NutrientConsumptionStatisticsComponent},

    ],
  }
];

import { Routes } from '@angular/router';
import { PersonaPageComponent } from '../pages/persona-page/persona-page.component';
import { LoginPageComponent } from '../pages/login-page/login-page.component';
import { NavbarComponent } from '../components/navbar/navbar.component';

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'login'
      },
      {
        path: 'login',
        component: LoginPageComponent
      },
      {
        path: 'home',
        component: NavbarComponent,
        // canActivateChild: [AuthGuard],
        children: [
          {
            path: 'persona',
            component: PersonaPageComponent,
            outlet: 'navBar'
          },
          {
            path: 'logs',
            component: PersonaPageComponent,
            outlet: 'navBar'
          },
        ]
      },
      {
        path: '**',
        redirectTo: 'login'
      }
];

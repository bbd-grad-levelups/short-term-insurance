import { Component } from '@angular/core';
import { LoginPanelComponent } from '../../components/login-panel/login-panel.component';
import { LogoComponent } from '../../components/logo/logo.component';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [
    LoginPanelComponent,
    LogoComponent
  ],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent {

}

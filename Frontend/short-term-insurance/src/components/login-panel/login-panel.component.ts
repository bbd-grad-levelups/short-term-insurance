import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { DOCUMENT } from "@angular/common";
import { environment } from "../../../environment";
import { PreventDoubleClick } from "../../directives/prevent-double-click.directive";
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-login-panel',
  standalone: true,
  imports: [
    PreventDoubleClick,
    MatButton,
  ],
  templateUrl: './login-panel.component.html',
  styleUrl: './login-panel.component.css'
})
export class LoginPanelComponent implements OnInit {
  disableButtons = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    @Inject(DOCUMENT) private document: Document,
  ) {}

  ngOnInit() {
    // already logged In
    if (sessionStorage.getItem('id_token')) {
      this.router.navigate(['/home']);
    }

    // login/signUp
    this.route.fragment.subscribe((fragments) => {
      if (!fragments) {
        return;
      }

      const fragment = fragments.split('&')[1]?.split('=');
      if (fragment[0] !== 'id_token') {
        return;
      }

      const token = fragment[1];
      if (token) {
        sessionStorage.setItem('id_token', token);
        this.router.navigate(['/home']);
      }
    });
  }

  private authorizationUrl = environment.authorizationUrl;

  loginOnSubmit() {
    this.document.location.href = this.authorizationUrl;
  }
}

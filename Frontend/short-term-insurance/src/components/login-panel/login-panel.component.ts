import {Component, Inject, OnInit} from '@angular/core';
import {LogoComponent} from "../logo/logo.component";
import {ActivatedRoute, Router, RouterLink} from "@angular/router";
import {FormsModule, NgForm} from "@angular/forms";
import {BusinessService} from "../../services/business.service";
import {Business} from "../../models/business.model";
import {MatSnackBar} from "@angular/material/snack-bar";
import {MatError, MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {MatOption} from "@angular/material/core";
import {MatSelect} from "@angular/material/select";
import {DOCUMENT, NgIf, Location} from "@angular/common";
import {MatIconModule} from "@angular/material/icon";
import {environment} from "../../../environment";
import {finalize} from "rxjs";
import {MatButton} from "@angular/material/button";
import {PreventDoubleClick} from "../../directives/prevent-double-click.directive";

@Component({
  selector: 'app-login-panel',
  standalone: true,
  imports: [
    LogoComponent,
    RouterLink,
    FormsModule,
    MatInput,
    MatError,
    MatFormField,
    MatLabel,
    MatOption,
    MatSelect,
    NgIf,
    MatIconModule,
    MatButton,
    PreventDoubleClick,
  ],
  templateUrl: './login-panel.component.html',
  styleUrl: './login-panel.component.css'
})
export class LoginPanelComponent implements OnInit{
  businessName: string = '';
  disableButtons = false;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private businessService: BusinessService,
              private snackBar: MatSnackBar,
              @Inject(DOCUMENT) private document: Document,
              private location: Location,

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
        // set token - which means they either clicked login or register
        sessionStorage.setItem('id_token', token);

        const name = sessionStorage.getItem('registrationBusinessName')
        if (name) {
          // user clicked register
          this.registerBusiness(name);
        } else {
          // user clicked login
          this.verifyAndLoginBusiness();
        }
      }
    });
  }

  private authorizationUrl = environment.authorizationUrl;
  registerOnSubmit(registrationForm: NgForm) {
    if (registrationForm.form.valid) {
      sessionStorage.setItem('registrationBusinessName', this.businessName);
      this.document.location.href = this.authorizationUrl;
    } else {
      this.snackBar.open('Please Enter Business Name.', 'Ok', {"duration": 4000});
    }
  }

  loginOnSubmit() {
    this.document.location.href = this.authorizationUrl;
  }

  private registerBusiness(name: string) {
    if (!this.disableButtons) {

      this.disableButtons = true;

      const request: Business = {
        name: name
      }

      this.businessService
        .registerBusiness(request)
        .pipe(
          finalize(() => {
            this.disableButtons = false;
          })
        )
        .subscribe({
          next: response => {
            if (response.success) {
              this.snackBar.open('Logging In...', 'Ok', {"duration": 2000});

              setTimeout(() => {
                sessionStorage.setItem('businessName', response.data)
                this.router.navigate(['/home', {outlets: {'navBar': ['goals']}}]);
              }, 2000);
            }
          },
          error: () => {
            this.clearToken();
            // TODO: general failure || account exists
            this.snackBar.open('Registration Failed. Account Already Exists.', 'Ok', {"duration": 4000});
          },

        });
    }
  }

  private clearToken() {
    const pathWithoutHash = this.location.path(false);
    this.location.replaceState(pathWithoutHash);
    sessionStorage.clear();
  }

  private verifyAndLoginBusiness() {
    if (!this.disableButtons) {

      this.disableButtons = true;

      this.businessService
        .getBusiness()
        .pipe(
          finalize(() => {
            this.disableButtons = false;
          })
        )
        .subscribe({
          next: response => {
            if (response.success) {
              // business exists
              sessionStorage.setItem('businessName', response.data?.name)
              this.router.navigate(['/home', {outlets: {'navBar': ['goals']}}]);
            }
          },
          error: () => {
            // business does not exist
            this.snackBar.open('Login Failed. Please Register First.', 'Ok', {"duration": 4000});
            this.clearToken();
          },
        })
    }
  }
}

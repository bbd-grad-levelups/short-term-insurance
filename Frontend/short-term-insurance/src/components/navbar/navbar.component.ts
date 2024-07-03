import { AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { MatListModule } from "@angular/material/list";
import { MatSidenav, MatSidenavModule } from "@angular/material/sidenav";
import { NavigationEnd, Router, RouterLink, RouterModule, RouterOutlet } from "@angular/router";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatToolbarModule } from "@angular/material/toolbar";
import { SlicePipe } from "@angular/common";
import { BreakpointObserver } from "@angular/cdk/layout";
import { LogoComponent } from '../logo/logo.component';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    LogoComponent,
    RouterOutlet,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    RouterLink,
    RouterModule,
    SlicePipe,
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit, AfterViewInit {

  StoreName: string = sessionStorage.getItem('businessName') ?? '';

  @ViewChild(MatSidenav) sidenav!: MatSidenav;
  isMobile: boolean = false;

  constructor(
    private router: Router,
    private observer: BreakpointObserver,
    private changeDetectorRef: ChangeDetectorRef,
  ) { }

  ngAfterViewInit() {
    this.changeDetectorRef.detectChanges();
  }

  ngOnInit() {
    this.refreshSetup();
    this.mobileSetup();
  }

  toggleMenu() {
    if (this.isMobile) {
      this.sidenav.toggle();
    }
  }

  private refreshSetup() {
    console.log('Last visited route:', sessionStorage.getItem('lastVisitedRoute'));
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        const currentRoute = this.router.url;
        sessionStorage.setItem('lastVisitedRoute', currentRoute);
      }
    });

    const lastVisitedRoute = sessionStorage.getItem('lastVisitedRoute');
    if (lastVisitedRoute === '/home') {
      this.router.navigate(['/home', { outlets: { 'navBar': ['persona'] } }]);
    }
    else if (lastVisitedRoute) {
      this.router.navigateByUrl(lastVisitedRoute);
    }
  }

  private mobileSetup() {
    this.observer
      .observe(['(max-width: 800px)'])
      .subscribe((screenSize) => {
        this.isMobile = screenSize.matches;
      });
  }

  logout() {
    sessionStorage.clear();
    this.router.navigateByUrl('/login');
  }
}

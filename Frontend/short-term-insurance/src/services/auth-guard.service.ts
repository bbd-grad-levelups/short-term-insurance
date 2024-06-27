import {inject, Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot} from "@angular/router";
import {MatSnackBar} from "@angular/material/snack-bar";

@Injectable({
  providedIn: 'root'
})
class PermissionsService {

  constructor(private router: Router, private snackBar: MatSnackBar) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const token = sessionStorage.getItem('id_token')

    if (token) {
      return true;
    }

    this.snackBar.open(
      'No Login Token Found.',
      'Ok',
      {"duration": 5000}
    );

    setTimeout(() => {
      sessionStorage.clear();
      this.router.navigate(['login']);
    }, 500);

    return false;

  }
}

export const AuthGuard: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean => {
  return inject(PermissionsService).canActivate(next, state);
}

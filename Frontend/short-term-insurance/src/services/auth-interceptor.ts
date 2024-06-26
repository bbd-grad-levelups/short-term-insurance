import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {Observable, tap} from "rxjs";
import {Injectable} from "@angular/core";
import {Router} from "@angular/router";
import {MatSnackBar} from "@angular/material/snack-bar";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private router: Router, private snackBar: MatSnackBar) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const userToken = sessionStorage.getItem('id_token')

    const modifiedReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${userToken}`),
    });

    return next.handle(modifiedReq)
      .pipe(
        tap({
          error: (error) => {
            if (error instanceof HttpErrorResponse) {
              switch (error.status) {
                case 401: {
                  this.snackBar.open(
                    'Session Expired.',
                    'Ok',
                    {"duration": 1000}
                  );

                  setTimeout(() => {
                    sessionStorage.clear();
                    this.router.navigate(['login']);
                  }, 500);

                  break;
                }
                case 500: {
                  const message = error?.error?.message ?? 'Encountered Unexpected Error.'
                  this.snackBar.open(
                    message,
                    'Ok',
                    {"duration": 3000}
                  );
                }
              }
            }
          }
        })
      );
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { delay } from 'rxjs/operators';
import { environment } from '../../environment';
import { ApiResponse } from '../models/api-response.model';
import { Logs } from '../models/logs.model';

@Injectable({
  providedIn: 'root'
})
export class LogsService {

  private baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getLogs(page: number): Observable<ApiResponse<Logs>> {
    const mockData: ApiResponse<Logs[]> = {
      success: true,
      message: "Mock data",
      data: [
        { date: "2024/06/28", message: "INFO  User 'johndoe' logged in from IP 192.168.8.4"},
        { date: "2024/06/29", message: "ERROR   User 'ivanC' has insufficient funds to make a claim"},
        { date: "2024/06/30", message: "INFO  User 'gerniV' logged out successfully"},
      ]
    };

    return of(mockData).pipe(delay(1000));
  }
}

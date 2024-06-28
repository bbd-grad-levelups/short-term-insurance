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
        { date: "2024/06/28", message: "This is a temp message"},
        { date: "2024/06/29", message: "This is another temp message"},
        { date: "2024/06/30", message: "This is also a temp message"},
      ]
    };

    return of(mockData).pipe(delay(1000));
  }
}

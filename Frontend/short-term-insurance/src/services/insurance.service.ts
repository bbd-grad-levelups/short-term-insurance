import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { environment } from '../../environment';
import { Persona } from '../models/persona.model';
import { Logs } from '../models/logs.model';

@Injectable({
  providedIn: 'root'
})
export class InsuranceService {

  private baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getPersonas(page: number): Observable<Persona[]> {
    return this.httpClient.get<Persona[]>(
      `${this.baseUrl}/api/personas?page=${page}`
    );
  }

  getLogs(beginDate: Date, endDate: Date, page: number): Observable<Logs[]> {
    return this.httpClient.get<Logs[]>(
      `${this.baseUrl}/api/log?beginDate=${beginDate}&endDate=${endDate}&page=${page}`
    );
  }
}

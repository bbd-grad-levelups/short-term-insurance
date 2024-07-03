import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable, map, of } from "rxjs";
import { environment } from '../../environment';
import { Persona } from '../models/persona.model';
import { Logs } from '../models/logs.model';
import { Pagination } from '../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class InsuranceService {

  private baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getPersonas(page: number): Observable<Pagination<Persona>> {
    return this.httpClient.get<any>(
      `${this.baseUrl}/api/personas?page=${page}`
    ).pipe(
      map((response) => {
        return {
          data: response.personas.map((persona: any) => {
            return {
              personaId: persona.personaId,
              electronics: persona.electronics,
              blacklisted: this.isBlacklisted(persona.lastPaymentDate)
            }
          }),
          page: response.page,
          availablePages: response.availablePages,
          pageSize: response.pageSize
        }
      })
    )
  }

  getLogs(beginDate: String, endDate: String, page: number): Observable<Pagination<Logs>> {
    return this.httpClient.get<any>(
      `${this.baseUrl}/api/log?beginDate=${beginDate}&endDate=${endDate}&page=${page}`
    ).pipe(
      map((response) => {
        return {
          data: response.logs,
          page: response.page,
          availablePages: response.availablePages,
          pageSize: response.pageSize
        }
      })
    )
  }

  isBlacklisted(lastPaymentDate: string): boolean {
    const BUFFER = 30 * 24 * 60 * 60 * 1000;
    const lastPayment = new Date(lastPaymentDate.replace(/\|/g, "/"));
    const currentDate = new Date();

    const diffTime = currentDate.getTime() - lastPayment.getTime();
    if (diffTime > BUFFER) {
      return true;
    }
    return false;
  }
}

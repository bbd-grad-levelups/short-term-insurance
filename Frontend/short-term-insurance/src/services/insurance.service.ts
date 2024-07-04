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
      `${this.baseUrl}/api/personas?page=${page}&pageSize=10`
    ).pipe(
      map((response) => {
        return {
          data: response.personas.map((persona: any) => {
            return {
              personaId: persona.personaId,
              electronics: persona.electronics,
              blacklisted: persona.lastPaymentDate.replace(/\|/g, '/'),
              debitOrderId: persona.debitOrderId
            }
          }),
          page: response.page,
          availablePages: response.availablePages,
          pageSize: response.pageSize
        }
      })
    )
  }

  getLogs(beginDate: string, endDate: string, page: number): Observable<Pagination<Logs>> {
    return this.httpClient.get<any>(
      `${this.baseUrl}/api/log?beginDate=${beginDate}&endDate=${endDate}&page=${page}&pageSize=10`
    ).pipe(
      map((response) => {
        return {
          data: response.logs.map((log: any) => {
            return {
              timeStamp: log.timeStamp.replace(/\|/g, '/'),
              message: log.message
            }
          }),
          page: response.page,
          availablePages: response.availablePages,
          pageSize: response.pageSize
        }
      })
    )
  }
}

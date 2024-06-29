import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { delay, map } from 'rxjs/operators';
import { environment } from '../../environment';
import { Persona } from '../models/persona.model';

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
}

import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable, of} from "rxjs";
import { environment } from '../../environment';
import { ApiResponse } from '../models/api-response.model';
import { Persona } from '../models/persona.model';

@Injectable({
  providedIn: 'root'
})
export class InsuranceService {

  private baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getPersonas(page: number): Observable<ApiResponse<Persona>> {
    const mockData: ApiResponse<Persona[]> = {
        success: true,
        message: "Mock data",
        data: [
          { persona: "John Doe " + page },
          { persona: "Jane Doe" },
          { persona: "Alice Smith" }
        ]
      };
  
      return of(mockData);
    }
}

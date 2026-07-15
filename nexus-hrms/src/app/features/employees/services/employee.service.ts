import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private apiUrl = 'https://localhost:5001/api/Employee/getEmployee';

  constructor(private http: HttpClient) {}

  getEmployees(): Observable<any> {
    return this.http.post(this.apiUrl, {});
  }
}

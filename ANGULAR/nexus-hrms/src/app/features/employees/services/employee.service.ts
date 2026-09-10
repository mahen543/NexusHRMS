import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../auth/services/auth.service';
import { CreateEmployeeRequest, Employee, UpdateEmployeeRequest } from '../models/employee.model';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private readonly baseUrl = 'https://localhost:5001/api/Employee';

  // Helper method to get Auth Headers automatically
  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  /**
   * GET: Fetch all employees
   */
  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.baseUrl}/getEmployee`, { 
      headers: this.getAuthHeaders() 
    });
  }

  /**
   * GET BY ID: Fetch single employee details
   */
  getEmployeeById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/getEmployee/${id}`, { 
      headers: this.getAuthHeaders() 
    });
  }

  /**
   * POST: Create/Insert a new employee
   */
  createEmployee(payload: CreateEmployeeRequest): Observable<Employee> {
    return this.http.post<Employee>(`${this.baseUrl}/createEmployee`, payload, { 
      headers: this.getAuthHeaders() 
    });
  }

  /**
   * PUT: Update an existing employee
   */
  updateEmployee(payload: UpdateEmployeeRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/updateEmployee`, payload, { 
      headers: this.getAuthHeaders() 
    });
  }

  /**
   * DELETE: Delete an employee by ID
   */
  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/deleteEmployee/${id}`, { 
      headers: this.getAuthHeaders() 
    });
  }
}
import { Injectable, signal } from '@angular/core';
import { ChangePasswordRequest, LoginRequest, LoginResponse, UserSession } from '../models/auth.model';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly authUrl = 'https://localhost:5001/api/Auth';
  private readonly employeeUrl = 'https://localhost:5001/api/Employee';

  currentUser = signal<UserSession | null>(this.getUserFromToken());

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.authUrl}/login`, credentials).pipe(
      tap((response) => {
        if (response.token) {
          localStorage.setItem('auth_token', response.token);
          this.currentUser.set(this.getUserFromToken());
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    this.currentUser.set(null);
  }

  isAuthenticated(): boolean {
    return !!this.currentUser();
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  changePassword(payload: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.employeeUrl}/changePassword`, payload);
  }

  private getUserFromToken(): UserSession | null {
    const token = localStorage.getItem('auth_token');
    if (!token) return null;

    try {
      const payloadBase64 = token.split('.')[1];
      const decodedJson = atob(payloadBase64);
      const decoded = JSON.parse(decodedJson);

      const userIdClaim = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || decoded['nameidentifier'];
      const userNameClaim = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || decoded['name'];

      return {
        id: Number(userIdClaim) || 0,
        userName: userNameClaim || '',
        employeeGuid: decoded['EmployeeGuid'] || '',
        designationId: Number(decoded['DesignationId']) || 0,
        hierarchyLevel: Number(decoded['HierarchyLevel']) || 0,
      };
    } catch {
      return null;
    }
  }
}
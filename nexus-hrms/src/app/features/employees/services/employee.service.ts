import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../auth/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
private http = inject(HttpClient);
  private authService = inject(AuthService);

  getEmployees() {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.post('https://localhost:5001/api/Employee/getEmployee', {}, { headers });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private readonly TOKEN_KEY = 'auth_token';
  private loginUrl = 'https://localhost:5001/api/Auth/login';

  login(userName: string, password: string): Observable<string> {
    const body = {
      userName: userName,
      password: password
    };

    return this.http.post(this.loginUrl, body, { responseType: 'text' });
  }

// auth.service.ts
setToken(token: string): void {
  if (!token) return;

  let cleanToken = token.trim();

  try {
    // If token response is JSON like {"token": "eyJhb..."}, parse it out
    const parsed = JSON.parse(cleanToken);
    if (typeof parsed === 'object' && parsed !== null && parsed.token) {
      cleanToken = parsed.token;
    } else if (typeof parsed === 'string') {
      cleanToken = parsed;
    }
  } catch (e) {
    // String is standard JWT, handle basic quotes if present
    if (cleanToken.startsWith('"') && cleanToken.endsWith('"')) {
      cleanToken = cleanToken.slice(1, -1);
    }
  }

  localStorage.setItem(this.TOKEN_KEY, cleanToken);
}

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
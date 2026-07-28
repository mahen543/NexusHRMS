import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  console.log('--- INTERCEPTOR RUNNING ---');
  console.log('Target URL:', req.url);
  console.log('Retrieved Token:', token);

  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    console.log('Header Attached Successfully!');
    return next(clonedReq);
  }

  console.warn('NO TOKEN FOUND IN STORAGE!');
  return next(req);
};
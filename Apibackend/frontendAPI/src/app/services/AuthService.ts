import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { AuthResponse } from '../models/AuthResponse';
import { LoginModel } from '../models/LoginModel';
import { RegisterModel } from '../models/RegisterModel';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7055/api/auth';

   private currentUser: any = null;
  constructor(private http: HttpClient , private router: Router) {}

  login(loginData: LoginModel): Observable<AuthResponse> {
    
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, loginData);
  }
    
register(user: RegisterModel): Observable<string> {
  // On précise responseType: 'text' pour recevoir une string
  return this.http.post(`${this.apiUrl}/register`, user, { responseType: 'text' });
}

 saveToken(token: string) {
  localStorage.setItem('token', token);
}

getJwtToken(): string | null {
  return localStorage.getItem('token'); // doit correspondre à saveToken
}


    logout() {
    localStorage.removeItem('token'); // supprime token JWT
    this.router.navigate(['/login']); // redirige vers login
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

    // Stocker l’utilisateur
  setUser(user: any) {
    this.currentUser = user;
    localStorage.setItem('user', JSON.stringify(user));
  }

  getUser(): any {
    if (!this.currentUser) {
      const stored = localStorage.getItem('user');
      if (stored) {
        this.currentUser = JSON.parse(stored);
      }
    }
    return this.currentUser;
  }
}

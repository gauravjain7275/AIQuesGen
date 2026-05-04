import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, tap } from 'rxjs';
import { User } from '../shared/models/user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private API_URL = environment.apiUrl;

  currentUser$ = new BehaviorSubject<User | null>(null);

  constructor(private http: HttpClient) {
    const user = localStorage.getItem('user');
    if (user) {
      this.currentUser$.next(JSON.parse(user));
    }
   }

  login(email: string, password: string) {
    return this.http.post<User>(`${this.API_URL}/api/auth/login`, { email, password })
      .pipe(tap(user => this.saveUser(user)));
  }

  signup(data: any) {
    return this.http.post<User>(`${this.API_URL}/api/auth/signup`, data)
      .pipe(tap(user => this.saveUser(user)));
  }

  private saveUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    localStorage.setItem('token', user.token);
    this.currentUser$.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser$.next(null);
  }

  get token() {
    const u = localStorage.getItem('user');
    if (!u) return null;
    return JSON.parse(u).token;
  }

  isLoggedIn(): boolean {
    return !!this.token;
  }

  getUser() {
  const user = localStorage.getItem('user');
  return user ? JSON.parse(user) : null;
}

getCurrentUserId(): number {
    const u = localStorage.getItem('user');
    if (!u) return 0;
    try { 
      const userObj = JSON.parse(u);
      const userId = userObj.user?.userId || 0;
      return userId; 
    } 
    catch 
    { 
      return 0; 
    }
  }
}
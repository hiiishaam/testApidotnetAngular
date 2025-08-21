import { Component } from '@angular/core';
import { LoginModel } from '../../../models/LoginModel';
import { AuthService } from '../../../services/AuthService';
import { Router, RouterModule } from '@angular/router';
import { AuthResponse } from '../../../models/AuthResponse';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserModel } from '../../../models/UserModel';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
loginData: LoginModel = { email: '', password: '' };
  errorMessage: string = '';
  loading: boolean = false;
  user?: UserModel;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    if (!this.loginData.email || !this.loginData.password) {
      this.errorMessage = 'Veuillez remplir tous les champs.';
      return;
    }

    this.loading = true;
    this.authService.login(this.loginData).subscribe({
      next: (response: AuthResponse) => {
        this.authService.saveToken(response.token);
        this.authService.setUser(response.user);
        this.user = response.user;
        this.loading = false;
        console.log(response);
        this.router.navigate(['/products']);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Email ou mot de passe incorrect';
        this.loading = false;
      }
    });
  }
}

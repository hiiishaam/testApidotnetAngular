import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RegisterModel } from '../../../models/RegisterModel';
import { AuthService } from '../../../services/AuthService';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  userData: RegisterModel = {
    fullName: '',
    email: '',
    password: '',
    role: 'user'
  };

  successMessage = '';
  errorMessage = '';
  loading = false;

  constructor(private authService: AuthService , private router: Router) {}

  register() {
  this.loading = true;
  this.errorMessage = '';
  this.successMessage = '';

  this.authService.register(this.userData).subscribe({
    next: (res: any) => {
      // Si la réponse est une string (texte brut)
      if (typeof res === 'string') {
        this.successMessage = res;
      } 
      // Si la réponse est un objet JSON avec une propriété message
      else if (res && res.message) {
        this.successMessage = res.message;
      } 
      // Fallback si la réponse est inattendue
      else {
        this.successMessage = 'Inscription réussie ✅';
      }

      this.loading = false;

      // Redirection vers login après 2 secondes
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 2000);
    },
    error: (err) => {
      // Vérifie si err.error est du texte ou un objet
      if (typeof err.error === 'string') {
        this.errorMessage = err.error;
      } else {
        this.errorMessage = err.error?.message || 'Erreur d\'inscription ❌';
      }
      this.loading = false;
    }
  });
}


}

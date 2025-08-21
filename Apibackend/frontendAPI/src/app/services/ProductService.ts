// product.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductModel } from '../models/ProductModel';
import { AuthService } from './AuthService';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'https://localhost:7055/api/products';

  constructor(private http: HttpClient, private authService: AuthService) {}

  // Fonction pour créer les headers avec le token
private getAuthHeaders(): HttpHeaders {
  const token = this.authService.getJwtToken();
  if (!token) {
    throw new Error('Token manquant. L’utilisateur doit se connecter.');
  }
  return new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}`
  });
}



  getAll(): Observable<ProductModel[]> {
    return this.http.get<ProductModel[]>(this.apiUrl, { headers: this.getAuthHeaders() });
  }

  getById(id: number): Observable<ProductModel> {
    return this.http.get<ProductModel>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }

  create(product: ProductModel): Observable<ProductModel> {
    return this.http.post<ProductModel>(this.apiUrl, product, { headers: this.getAuthHeaders() });
  }

  update(id: number, product: ProductModel): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, product, { headers: this.getAuthHeaders() });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }
}

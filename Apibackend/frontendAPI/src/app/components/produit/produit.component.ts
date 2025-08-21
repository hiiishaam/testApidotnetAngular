import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProductModel } from '../../models/ProductModel';
import { ProductService } from '../../services/ProductService';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/AuthService';
import { Subscription } from 'rxjs';
import { EventService } from '../../services/EventService';

@Component({
  selector: 'app-produit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './produit.component.html',
  styleUrls: ['./produit.component.css']
})
export class ProduitComponent implements OnInit, OnDestroy {

  products: ProductModel[] = [];
  paginatedProducts: ProductModel[] = [];
  selectedProduct: ProductModel = { id: 0, name: '', price: 0, description: '' };
  isEditing: boolean = false;
  errorMessage: string = '';

  // Pagination
  currentPage: number = 1;
  pageSize: number = 5;

  user: any = null;

  // 🔔 Subscriptions aux events
  private subscriptions: Subscription[] = [];

  constructor(
    private productService: ProductService,
    private authService: AuthService,
    private eventService: EventService // 👈 injecté
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.authService.logout();
      return;
    }
    this.user = this.authService.getUser();

    this.loadProducts();

    // 👉 démarrer la connexion SignalR
    this.eventService.startConnection();

    // 👉 écouter les events produits
    this.subscriptions.push(
      this.eventService.productCreated$.subscribe(event => {
        if (event) {
          console.log('📢 Produit créé:', event);
          this.loadProducts();
        }
      }),
      this.eventService.productUpdated$.subscribe(event => {
        if (event) {
          console.log('✏️ Produit mis à jour:', event);
          this.loadProducts();
        }
      }),
      this.eventService.productDeleted$.subscribe(event => {
        if (event) {
          console.log('🗑️ Produit supprimé:', event);
          this.loadProducts();
        }
      })
    );
  }

  ngOnDestroy(): void {
    // 👉 éviter fuites mémoire
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.eventService.stopConnection();
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data;
        this.updatePaginatedProducts();
      },
      error: (err) => this.errorMessage = 'Erreur lors du chargement des produits: ' + err
    });
  }

  updatePaginatedProducts(): void {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedProducts = this.products.slice(startIndex, endIndex);
  }

  totalPages(): number {
    return Math.ceil(this.products.length / this.pageSize);
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages()) {
      this.currentPage++;
      this.updatePaginatedProducts();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePaginatedProducts();
    }
  }

  selectProduct(product: ProductModel): void {
    this.selectedProduct = { ...product };
    this.isEditing = true;
  }

  clearSelection(): void {
    this.selectedProduct = { id: 0, name: '', price: 0, description: '' };
    this.isEditing = false;
    this.errorMessage = '';
  }

  saveProduct(): void {
    if (!this.selectedProduct.name || this.selectedProduct.price <= 0 || !this.selectedProduct.description) {
      this.errorMessage = 'Veuillez remplir tous les champs correctement.';
      return;
    }

    if (this.isEditing) {
      if (!this.selectedProduct.id) {
        this.errorMessage = 'Impossible de mettre à jour un produit sans ID.';
        return;
      }
      this.productService.update(this.selectedProduct.id, this.selectedProduct).subscribe({
        next: () => {
          this.loadProducts();
          this.clearSelection();
        },
        error: (err) => this.errorMessage = 'Erreur lors de la mise à jour: ' + err
      });
    } else {
      this.productService.create(this.selectedProduct).subscribe({
        next: () => {
          this.loadProducts();
          this.clearSelection();
        },
        error: (err) => this.errorMessage = 'Erreur lors de la création: ' + err
      });
    }
  }

  deleteProduct(id?: number): void {
    if (id === undefined) {
      this.errorMessage = 'Impossible de supprimer : ID manquant';
      return;
    }
    if (confirm('Voulez-vous vraiment supprimer ce produit ?')) {
      this.productService.delete(id).subscribe({
        next: () => this.loadProducts(),
        error: (err) => this.errorMessage = 'Erreur lors de la suppression: ' + err
      });
    }
  }

  logout(): void {
    if (confirm('Voulez-vous vraiment vous déconnecter ?')) {
      this.authService.logout();
    }
  }
}

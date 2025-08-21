// src/app/services/event.service.ts
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

// Tu peux créer des interfaces si tu veux typer
export interface ProductCreatedEvent {
  productId: number;
  name: string;
}

export interface ProductUpdatedEvent {
  productId: number;
  name: string;
  price: number;
}

export interface ProductDeletedEvent {
  productId: number;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private hubConnection?: signalR.HubConnection;
  private isConnected = false;

  private productCreatedSource = new BehaviorSubject<ProductCreatedEvent | null>(null);
  productCreated$ = this.productCreatedSource.asObservable();

  private productUpdatedSource = new BehaviorSubject<ProductUpdatedEvent | null>(null);
  productUpdated$ = this.productUpdatedSource.asObservable();

  private productDeletedSource = new BehaviorSubject<ProductDeletedEvent | null>(null);
  productDeleted$ = this.productDeletedSource.asObservable();

  startConnection() {
    if (this.isConnected) return; // évite double connexion

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7055/hubs/products') // ✅ URL backend
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.isConnected = true;
        console.log('✅ SignalR connecté');
        this.registerEventListeners();
      })
      .catch(err => console.error('❌ Erreur SignalR:', err));
  }

  private registerEventListeners() {
    if (!this.hubConnection) return;

    this.hubConnection.on('ProductCreated', (event: ProductCreatedEvent) => {
      console.log('📢 Event ProductCreated reçu:', event);
      this.productCreatedSource.next(event);
    });

    this.hubConnection.on('ProductUpdated', (event: ProductUpdatedEvent) => {
      console.log('✏️ Event ProductUpdated reçu:', event);
      this.productUpdatedSource.next(event);
    });

    this.hubConnection.on('ProductDeleted', (event: ProductDeletedEvent) => {
      console.log('🗑️ Event ProductDeleted reçu:', event);
      this.productDeletedSource.next(event);
    });
  }

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().then(() => {
        this.isConnected = false;
        console.log('⚡ SignalR déconnecté');
      });
    }
  }
}

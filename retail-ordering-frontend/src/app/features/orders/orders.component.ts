import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../core/models/order.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="orders-page">
      <div class="orders-container">
        <h1>My Orders</h1>

        @if (loading) {
          <div class="loading">Loading orders...</div>
        } @else if (orders.length === 0) {
          <div class="empty-orders">
            <div class="empty-icon">📦</div>
            <h2>No orders yet</h2>
            <p>Start shopping to see your orders here</p>
            <a routerLink="/products" class="btn-primary">Browse Products</a>
          </div>
        } @else {
          <div class="orders-list">
            @for (order of orders; track order.id) {
              <div class="order-card" [routerLink]="['/orders', order.id]">
                <div class="order-header">
                  <span class="order-id">Order #{{ order.id }}</span>
                  <span class="order-status" [class]="'status-' + order.status.toLowerCase()">
                    {{ order.status }}
                  </span>
                </div>
                <div class="order-body">
                  <div class="order-info">
                    <span class="order-date">{{ order.orderDate | date:'medium' }}</span>
                    <span class="order-items">{{ order.orderItems.length }} item(s)</span>
                  </div>
                  <div class="order-total">₹{{ order.totalAmount.toFixed(2) }}</div>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  loading = true;

  constructor(private orderService: OrderService, private router: Router, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.orderService.getOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => { this.loading = false; this.cdr.markForCheck(); }
    });
  }
}

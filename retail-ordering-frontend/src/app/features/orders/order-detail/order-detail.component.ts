import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../core/models/order.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="order-detail-page">
      <div class="order-detail-container">
        @if (loading) {
          <div class="loading">Loading order...</div>
        } @else if (!order) {
          <div class="error-state">
            <p>Order not found</p>
            <a routerLink="/orders" class="btn-primary">Back to Orders</a>
          </div>
        } @else {
          <a routerLink="/orders" class="btn-back">← Back to Orders</a>

          <div class="order-detail-header">
            <div>
              <h1>Order #{{ order.id }}</h1>
              <p class="order-date">Placed on {{ order.orderDate | date:'fullDate' }}</p>
            </div>
            <span class="order-status" [class]="'status-' + order.status.toLowerCase()">
              {{ order.status }}
            </span>
          </div>

          <div class="order-detail-grid">
            <div class="order-items-section">
              <h2>Items</h2>
              @for (item of order.orderItems; track item.id) {
                <div class="order-item">
                  <div class="item-details">
                    <h3>{{ item.productName }}</h3>
                    <p>Qty: {{ item.quantity }} × ₹{{ item.unitPrice.toFixed(2) }}</p>
                  </div>
                  <span class="item-total">₹{{ (item.unitPrice * item.quantity).toFixed(2) }}</span>
                </div>
              }
            </div>

            <div class="order-summary-section">
              <h2>Summary</h2>
              <div class="summary-row">
                <span>Subtotal</span>
                <span>₹{{ order.totalAmount.toFixed(2) }}</span>
              </div>
              <div class="summary-row">
                <span>Delivery</span>
                <span class="free">FREE</span>
              </div>
              <div class="summary-total">
                <strong>Total</strong>
                <strong>₹{{ order.totalAmount.toFixed(2) }}</strong>
              </div>

              <div class="delivery-info">
                <h3>Delivery Address</h3>
                <p>{{ order.deliveryAddress }}</p>
                @if (order.deliveryNotes) {
                  <h3>Notes</h3>
                  <p>{{ order.deliveryNotes }}</p>
                }
              </div>

              @if (order.status === 'Pending' || order.status === 'Confirmed') {
                <button class="btn-cancel" (click)="cancelOrder()" [disabled]="cancelling">
                  {{ cancelling ? 'Cancelling...' : 'Cancel Order' }}
                </button>
              }
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styleUrl: './order-detail.component.css'
})
export class OrderDetailComponent implements OnInit {
  order: Order | null = null;
  loading = true;
  cancelling = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.orderService.getOrderById(id).subscribe({
      next: (order) => {
        this.order = order;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  cancelOrder(): void {
    if (!this.order) return;
    this.cancelling = true;
    this.orderService.cancelOrder(this.order.id).subscribe({
      next: (order) => {
        this.order = order;
        this.cancelling = false;
        this.cdr.markForCheck();
      },
      error: () => { this.cancelling = false; this.cdr.markForCheck(); }
    });
  }
}

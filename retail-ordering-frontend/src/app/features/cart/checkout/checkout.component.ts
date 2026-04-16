import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CartService } from '../../../core/services/cart.service';
import { OrderService } from '../../../core/services/order.service';
import { CartItem } from '../../../core/models/cart.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="checkout-page">
      <div class="checkout-container">
        <h1>Checkout</h1>

        @if (orderSuccess) {
          <div class="success-card">
            <div class="success-icon">🎉</div>
            <h2>Order Placed Successfully!</h2>
            <p>Your order #{{ orderId }} has been confirmed.</p>
            <p>We'll prepare your order right away!</p>
            <button class="btn-primary" (click)="goToOrders()">View My Orders</button>
          </div>
        } @else {
          <div class="checkout-layout">
            <!-- Order Summary -->
            <div class="order-summary">
              <h2>Order Summary</h2>
              @for (item of cartItems; track item.id) {
                <div class="summary-item">
                  <span>{{ item.product?.name }} × {{ item.quantity }}</span>
                  <span>₹{{ ((item.product?.price ?? 0) * item.quantity).toFixed(2) }}</span>
                </div>
              }
              <div class="summary-divider"></div>
              <div class="summary-total">
                <strong>Total</strong>
                <strong>₹{{ cartTotal.toFixed(2) }}</strong>
              </div>
            </div>

            <!-- Delivery Form -->
            <div class="delivery-form">
              <h2>Delivery Details</h2>
              @if (error) {
                <div class="alert alert-error">{{ error }}</div>
              }
              <form [formGroup]="checkoutForm" (ngSubmit)="placeOrder()">
                <div class="form-group">
                  <label for="address">Delivery Address</label>
                  <textarea id="address" formControlName="deliveryAddress" rows="3"
                    placeholder="Enter your full delivery address..."
                    [class.invalid]="submitted && f['deliveryAddress'].errors"></textarea>
                  @if (submitted && f['deliveryAddress'].errors) {
                    <span class="error-msg">Delivery address is required</span>
                  }
                </div>
                <div class="form-group">
                  <label for="notes">Delivery Notes (optional)</label>
                  <textarea id="notes" formControlName="deliveryNotes" rows="2"
                    placeholder="Any special instructions?"></textarea>
                </div>
                <button type="submit" class="btn-place-order" [disabled]="loading">
                  {{ loading ? 'Placing Order...' : 'Place Order — ₹' + cartTotal.toFixed(2) }}
                </button>
              </form>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit {
  checkoutForm: FormGroup;
  cartItems: CartItem[] = [];
  loading = false;
  error = '';
  submitted = false;
  orderSuccess = false;
  orderId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.checkoutForm = this.fb.group({
      deliveryAddress: ['', Validators.required],
      deliveryNotes: ['']
    });
  }

  ngOnInit(): void {
    this.cartService.cartItems$.subscribe(items => { this.cartItems = items; this.cdr.markForCheck(); });
    this.cartService.getCart().subscribe();
  }

  get f() { return this.checkoutForm.controls; }

  get cartTotal(): number {
    return this.cartItems.reduce((sum, i) => sum + (i.product?.price ?? 0) * i.quantity, 0);
  }

  placeOrder(): void {
    this.submitted = true;
    this.error = '';
    if (this.checkoutForm.invalid) return;

    this.loading = true;
    this.orderService.createOrder({
      deliveryAddress: this.f['deliveryAddress'].value,
      deliveryNotes: this.f['deliveryNotes'].value
    }).subscribe({
      next: (order) => {
        this.orderId = order.id;
        this.orderSuccess = true;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to place order. Please try again.';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  goToOrders(): void {
    this.router.navigate(['/orders']);
  }
}

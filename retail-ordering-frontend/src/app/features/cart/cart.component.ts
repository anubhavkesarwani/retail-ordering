import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { CartItem } from '../../core/models/cart.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="cart-page">
      <div class="cart-container">
        <h1>Shopping Cart</h1>

        @if (loading) {
          <div class="loading">Loading cart...</div>
        } @else if (cartItems.length === 0) {
          <div class="empty-cart">
            <div class="empty-icon">🛒</div>
            <h2>Your cart is empty</h2>
            <p>Add some delicious items to get started!</p>
            <a routerLink="/products" class="btn-primary">Browse Products</a>
          </div>
        } @else {
          <div class="cart-layout">
            <div class="cart-items">
              @for (item of cartItems; track item.id) {
                <div class="cart-item">
                  <div class="item-image">
                    @if (item.product?.imageUrl) {
                      <img [src]="item.product?.imageUrl" [alt]="item.product?.name" />
                    } @else {
                      <div class="item-placeholder">🍕</div>
                    }
                  </div>
                  <div class="item-details">
                    <h3>{{ item.product?.name }}</h3>
                    <p class="item-price">₹{{ item.product?.price?.toFixed(2) }}</p>
                  </div>
                  <div class="item-qty">
                    <button (click)="updateQuantity(item, item.quantity - 1)" [disabled]="item.quantity <= 1">−</button>
                    <span>{{ item.quantity }}</span>
                    <button (click)="updateQuantity(item, item.quantity + 1)">+</button>
                  </div>
                  <div class="item-total">
                    ₹{{ ((item.product?.price ?? 0) * item.quantity).toFixed(2) }}
                  </div>
                  <button class="btn-remove" (click)="removeItem(item.id)">✕</button>
                </div>
              }
            </div>

            <div class="cart-summary">
              <h2>Order Summary</h2>
              <div class="summary-row">
                <span>Subtotal ({{ totalItems }} items)</span>
                <span>₹{{ cartTotal.toFixed(2) }}</span>
              </div>
              <div class="summary-row">
                <span>Delivery</span>
                <span class="free">FREE</span>
              </div>
              <div class="summary-total">
                <span>Total</span>
                <span>₹{{ cartTotal.toFixed(2) }}</span>
              </div>
              <button class="btn-checkout" (click)="checkout()">
                Proceed to Checkout →
              </button>
              <button class="btn-clear" (click)="clearCart()">Clear Cart</button>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  loading = true;

  constructor(
    private cartService: CartService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.cartService.getCart().subscribe({
      next: (items) => {
        this.cartItems = items;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => { this.loading = false; this.cdr.markForCheck(); }
    });

    this.cartService.cartItems$.subscribe(items => { this.cartItems = items; this.cdr.markForCheck(); });
  }

  get totalItems(): number {
    return this.cartItems.reduce((sum, i) => sum + i.quantity, 0);
  }

  get cartTotal(): number {
    return this.cartItems.reduce((sum, i) => sum + (i.product?.price ?? 0) * i.quantity, 0);
  }

  updateQuantity(item: CartItem, newQty: number): void {
    if (newQty < 1) return;
    this.cartService.updateCartItem(item.id, { quantity: newQty }).subscribe();
  }

  removeItem(id: number): void {
    this.cartService.removeFromCart(id).subscribe();
  }

  clearCart(): void {
    this.cartService.clearCart().subscribe();
  }

  checkout(): void {
    this.router.navigate(['/checkout']);
  }
}

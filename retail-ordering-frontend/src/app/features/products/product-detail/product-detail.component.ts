import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CartService } from '../../../core/services/cart.service';
import { AuthService } from '../../../core/services/auth.service';
import { Product } from '../../../core/models/product.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="product-detail-page">
      @if (loading) {
        <div class="loading-spinner">Loading...</div>
      } @else if (error || !product) {
        <div class="error-state">
          <p>{{ error || 'Product not found' }}</p>
          <button (click)="goBack()">Go Back</button>
        </div>
      } @else {
        <div class="product-detail-container">
          <button class="btn-back" (click)="goBack()">← Back to Products</button>

          <div class="product-detail-content">
            <div class="product-detail-image">
              @if (product.imageUrl) {
                <img [src]="product.imageUrl" [alt]="product.name" />
              } @else {
                <div class="image-placeholder">🍕</div>
              }
            </div>

            <div class="product-detail-info">
              <span class="product-category">{{ product.category?.name }}</span>
              <h1>{{ product.name }}</h1>
              <p class="product-description">{{ product.description }}</p>

              <div class="price-section">
                <span class="price">₹{{ product.price.toFixed(2) }}</span>
                @if (product.stockQuantity > 0) {
                  <span class="stock-badge in-stock">In Stock ({{ product.stockQuantity }})</span>
                } @else {
                  <span class="stock-badge out-stock">Out of Stock</span>
                }
              </div>

              @if (product.stockQuantity > 0 && product.isAvailable) {
                <div class="quantity-section">
                  <label>Quantity</label>
                  <div class="qty-controls">
                    <button (click)="decreaseQty()">−</button>
                    <span>{{ quantity }}</span>
                    <button (click)="increaseQty()">+</button>
                  </div>
                </div>

                <button class="btn-add-cart-large" (click)="addToCart()">
                  🛒 Add to Cart — ₹{{ (product.price * quantity).toFixed(2) }}
                </button>
              }

              @if (added) {
                <div class="success-toast">✅ Added to cart!</div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styleUrl: './product-detail.component.css'
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  loading = true;
  error = '';
  quantity = 1;
  added = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.product = product;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Product not found';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  decreaseQty(): void {
    if (this.quantity > 1) this.quantity--;
  }

  increaseQty(): void {
    if (this.product && this.quantity < this.product.stockQuantity) this.quantity++;
  }

  addToCart(): void {
    if (!this.authService.isAuthenticated) {
      this.router.navigate(['/login']);
      return;
    }
    if (!this.product) return;
    this.cartService.addToCart({ productId: this.product.id, quantity: this.quantity }).subscribe({
      next: () => {
        this.added = true;
        this.cdr.markForCheck();
        setTimeout(() => { this.added = false; this.cdr.markForCheck(); }, 3000);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/products']);
  }
}

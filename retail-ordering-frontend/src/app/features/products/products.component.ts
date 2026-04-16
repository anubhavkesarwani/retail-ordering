import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ProductService } from '../../core/services/product.service';
import { CartService } from '../../core/services/cart.service';
import { AuthService } from '../../core/services/auth.service';
import { Product, Brand, Category, Packaging } from '../../core/models/product.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="products-page">
      <div class="products-header">
        <h1>Our Menu</h1>
        <p>Fresh pizzas, cold drinks, and artisan breads</p>
      </div>

      <div class="products-layout">
        <!-- Sidebar Filters -->
        <aside class="filters-sidebar">
          <h2>Filters</h2>

          <div class="filter-section">
            <h3>Brands</h3>
            <label class="filter-option">
              <input type="radio" name="brand" [value]="null" [(ngModel)]="selectedBrandId" (change)="filterProducts()" />
              All Brands
            </label>
            @for (brand of brands; track brand.id) {
              <label class="filter-option">
                <input type="radio" name="brand" [value]="brand.id" [(ngModel)]="selectedBrandId" (change)="filterProducts()" />
                {{ brand.name }}
              </label>
            }
          </div>

          <div class="filter-section">
            <h3>Packaging</h3>
            <label class="filter-option">
              <input type="radio" name="packaging" [value]="null" [(ngModel)]="selectedPackagingId" (change)="filterProducts()" />
              All Packaging
            </label>
            @for (pkg of packagings; track pkg.id) {
              <label class="filter-option">
                <input type="radio" name="packaging" [value]="pkg.id" [(ngModel)]="selectedPackagingId" (change)="filterProducts()" />
                {{ pkg.name }}
              </label>
            }
          </div>

          <div class="filter-section">
            <h3>Categories</h3>
            <label class="filter-option">
              <input type="radio" name="category" [value]="null" [(ngModel)]="selectedCategoryId" (change)="filterProducts()" />
              All Categories
            </label>
            @for (cat of categories; track cat.id) {
              <label class="filter-option">
                <input type="radio" name="category" [value]="cat.id" [(ngModel)]="selectedCategoryId" (change)="filterProducts()" />
                {{ cat.name }}
              </label>
            }
          </div>
        </aside>

        <!-- Products Grid -->
        <div class="products-content">
          @if (loading) {
            <div class="loading-grid">
              @for (i of [1,2,3,4,5,6]; track i) {
                <div class="product-skeleton"></div>
              }
            </div>
          } @else if (error) {
            <div class="error-state">
              <span>⚠️</span>
              <p>{{ error }}</p>
            </div>
          } @else if (filteredProducts.length === 0) {
            <div class="empty-state">
              <span>🍽️</span>
              <p>No products found</p>
            </div>
          } @else {
            <div class="products-grid">
              @for (product of filteredProducts; track product.id) {
                <div class="product-card">
                  <div class="product-image" [routerLink]="['/products', product.id]">
                    @if (product.imageUrl) {
                      <img [src]="product.imageUrl" [alt]="product.name" />
                    } @else {
                      <div class="product-image-placeholder">🍕</div>
                    }
                  </div>
                  <div class="product-info">
                    <span class="product-category">{{ product.category?.name }}</span>
                    <h3 [routerLink]="['/products', product.id]">{{ product.name }}</h3>
                    <p class="product-desc">{{ product.description }}</p>
                    <div class="product-footer">
                      <span class="product-price">₹{{ product.price.toFixed(2) }}</span>
                      @if (product.stockQuantity > 0 && product.isAvailable) {
                        <button class="btn-add-cart" (click)="addToCart(product)">
                          Add to Cart
                        </button>
                      } @else {
                        <span class="out-of-stock">Out of Stock</span>
                      }
                    </div>
                  </div>
                </div>
              }
            </div>
          }
        </div>
      </div>
    </div>
  `,
  styleUrl: './products.component.css'
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  brands: Brand[] = [];
  categories: Category[] = [];
  packagings: Packaging[] = [];
  selectedBrandId: number | null = null;
  selectedCategoryId: number | null = null;
  selectedPackagingId: number | null = null;
  loading = true;
  error = '';

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    public authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.productService.getAllProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.filteredProducts = products;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = 'Failed to load products';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });

    this.productService.getBrands().subscribe({ next: (b) => { this.brands = b; this.cdr.markForCheck(); } });
    this.productService.getCategories().subscribe({ next: (c) => { this.categories = c; this.cdr.markForCheck(); } });
    this.productService.getPackagings().subscribe({ next: (p) => { this.packagings = p; this.cdr.markForCheck(); } });
  }

  filterProducts(): void {
    if (this.selectedBrandId) {
      this.productService.getProductsByBrand(this.selectedBrandId).subscribe({
        next: (p) => { this.filteredProducts = p; this.cdr.markForCheck(); }
      });
    } else if (this.selectedCategoryId) {
      this.productService.getProductsByCategory(this.selectedCategoryId).subscribe({
        next: (p) => { this.filteredProducts = p; this.cdr.markForCheck(); }
      });
    } else if (this.selectedPackagingId) {
      this.productService.getProductsByPackaging(this.selectedPackagingId).subscribe({
        next: (p) => { this.filteredProducts = p; this.cdr.markForCheck(); }
      });
    } else {
      this.filteredProducts = this.products;
    }
  }

  addToCart(product: Product): void {
    if (!this.authService.isAuthenticated) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/products' } });
      return;
    }
    this.cartService.addToCart({ productId: product.id, quantity: 1 }).subscribe();
  }
}

import { Component, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';
import { CartItem } from '../../../core/models/cart.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  cartCount = 0;

  constructor(
    public authService: AuthService,
    private cartService: CartService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.cartService.cartItems$.subscribe((items: CartItem[]) => {
      this.cartCount = items.reduce((sum: number, item: CartItem) => sum + item.quantity, 0);
      this.cdr.markForCheck();
    });
  }

  navigateToCart(): void {
    if (this.authService.isAuthenticated) {
      this.router.navigate(['/cart']);
    } else {
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/cart' } });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/home']);
  }
}

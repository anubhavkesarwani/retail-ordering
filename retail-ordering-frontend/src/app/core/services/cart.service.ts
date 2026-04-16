import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { CartItem, AddToCartRequest, UpdateCartRequest } from '../models/cart.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = `${environment.apiUrl}/cart`;
  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  public cartItems$ = this.cartItemsSubject.asObservable();

  constructor(private http: HttpClient) {}

  getCart(): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(this.apiUrl).pipe(
      tap(items => this.cartItemsSubject.next(items))
    );
  }

  addToCart(request: AddToCartRequest): Observable<CartItem> {
    return this.http.post<CartItem>(`${this.apiUrl}/items`, request).pipe(
      tap(() => this.refreshCart())
    );
  }

  updateCartItem(id: number, request: UpdateCartRequest): Observable<CartItem> {
    return this.http.put<CartItem>(`${this.apiUrl}/items/${id}`, request).pipe(
      tap(() => this.refreshCart())
    );
  }

  removeFromCart(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${id}`).pipe(
      tap(() => this.refreshCart())
    );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(this.apiUrl).pipe(
      tap(() => this.cartItemsSubject.next([]))
    );
  }

  private refreshCart(): void {
    this.getCart().subscribe();
  }

  get cartCount(): number {
    return this.cartItemsSubject.value.reduce((sum, item) => sum + item.quantity, 0);
  }

  get cartTotal(): number {
    return this.cartItemsSubject.value.reduce(
      (sum, item) => sum + (item.product?.price || 0) * item.quantity,
      0
    );
  }
}

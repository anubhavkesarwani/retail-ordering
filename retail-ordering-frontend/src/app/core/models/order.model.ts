import { Product } from './product.model';

export type OrderStatus = 'Pending' | 'Confirmed' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';

export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  product?: Product;
}

export interface Order {
  id: number;
  userId: number;
  orderDate: string;
  totalAmount: number;
  status: OrderStatus;
  deliveryAddress: string;
  deliveryNotes?: string;
  orderItems: OrderItem[];
}

export interface CreateOrderRequest {
  deliveryAddress: string;
  deliveryNotes?: string;
}

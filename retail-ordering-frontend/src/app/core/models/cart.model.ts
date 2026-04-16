export interface CartItem {
  id: number;
  productId: number;
  quantity: number;
  product?: CartProduct;
}

export interface CartProduct {
  id: number;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  stockQuantity: number;
  isAvailable: boolean;
}

export interface AddToCartRequest {
  productId: number;
  quantity: number;
}

export interface UpdateCartRequest {
  quantity: number;
}

export interface Brand {
  id: number;
  name: string;
  description?: string;
  imageUrl?: string;
  isActive: boolean;
}

export interface Category {
  id: number;
  name: string;
  description?: string;
  brandId: number;
  brand?: Brand;
}

export interface Packaging {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  packagingId?: number;
  imageUrl?: string;
  stockQuantity: number;
  isAvailable: boolean;
  category?: Category;
  packaging?: Packaging;
}
